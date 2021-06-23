using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hippo.Models;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tomlyn;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using Hippo.Schedulers;
using Hippo.Repositories;
using Microsoft.Extensions.Logging;
using Hippo.Tasks;
using System.Threading;

namespace Hippo.Controllers
{
    [Authorize]
    public class AppController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Account> _userManager;
        private readonly ITaskQueue<ChannelReference> _channelsToReschedule;

        public AppController(IUnitOfWork unitOfWork, UserManager<Account> userManager, ITaskQueue<ChannelReference> channelsToReschedule, ILogger<AppController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this._channelsToReschedule = channelsToReschedule;
        }

        public IActionResult Index()
        {
            TraceMethodEntry();

            return View(_unitOfWork.Applications.ListApplications());
        }

        public IActionResult Details(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            var a = _unitOfWork.Applications.GetApplicationById(id);
            LogIfNotFound(a, id);

            if (a == null)
            {
                return NotFound();
            }

            return View(a);
        }

        public IActionResult New()
        {
            TraceMethodEntry();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(AppNewForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                await _unitOfWork.Applications.AddNew(new Application
                {
                    Name = form.Name,
                    Owner = await _userManager.FindByNameAsync(User.Identity.Name),
                });
                await _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }

        public IActionResult Edit(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            var a = _unitOfWork.Applications.GetApplicationById(id);
            LogIfNotFound(a, id);

            if (a == null)
            {
                return NotFound();
            }

            var vm = new AppEditForm
            {
                Id = a.Id,
                Name = a.Name,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] AppEditForm form)
        {
            TraceMethodEntry(WithArgs(id, form));

            if (id != form.Id)
            {
                LogIdMismatch("application", id, form.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var a = _unitOfWork.Applications.GetApplicationById(id);
                    LogIfNotFound(a, id);

                    if (a == null)
                    {
                        return NotFound();
                    }

                    a.Name = form.Name;
                    _unitOfWork.Applications.Update(a);
                    await _unitOfWork.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWork.Applications.ApplicationExistsById(form.Id))
                    {
                        _logger.LogWarning($"Edit: concurrency error updating {form.Id}: no longer exists");
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError($"Edit: concurrency error updating {form.Id}");
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }

        public IActionResult Delete(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            var a = _unitOfWork.Applications.GetApplicationById(id);
            LogIfNotFound(a, id);

            if (a == null)
            {
                return NotFound();
            }

            return View(a);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            _unitOfWork.Applications.DeleteApplicationById(id);
            await _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Release(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            var a = _unitOfWork.Applications.GetApplicationById(id);
            var vm = new AppReleaseForm
            {
                Id = a.Id,
                Channels = a.Channels.AsSelectList(ch => ch.Name),
                RevisionSelectionStrategies = Converters.EnumValuesAsSelectList<ChannelRevisionSelectionStrategy>(),
                Revisions = a.Revisions.AsSelectList(r => r.RevisionNumber),
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(Guid id, AppReleaseForm form)
        {
            // TODO: this method is now a bit ill-named.  It is really
            // about changing the configuration of a channel.
            TraceMethodEntry(WithArgs(id, form));

            if (id != form.Id)
            {
                LogIdMismatch("application", id, form.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var application = _unitOfWork.Applications.GetApplicationById(id);
                var channel = _unitOfWork.Channels.GetChannelByName(application, form.SelectedChannelName);

                if (application == null || channel == null)
                {
                    LogIfNotFound(application, id);
                    LogIfNotFound(channel, form.SelectedChannelName);
                    return NotFound();
                }

                if (form.SelectedRevisionSelectionStrategy == Enum.GetName(ChannelRevisionSelectionStrategy.UseSpecifiedRevision))
                {
                    var revision = _unitOfWork.Revisions.GetRevisionByNumber(application, form.SelectedRevisionNumber);
                    if (revision == null)
                    {
                        LogIfNotFound(revision, form.SelectedRevisionNumber);
                        return NotFound();
                    }

                    channel.RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision;
                    channel.SpecifiedRevision = revision;
                }
                else if (form.SelectedRevisionSelectionStrategy == Enum.GetName(ChannelRevisionSelectionStrategy.UseRangeRule))
                {
                    var rule = form.SelectedRevisionRule;
                    if (string.IsNullOrWhiteSpace(rule))
                    {
                        _logger.LogError("Release: empty rule");
                        return BadRequest("rule was empty");  // TODO: this is a terrible way of handling it; await Ronan
                    }

                    channel.RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule;
                    channel.RangeRule = rule;
                }
                else
                {
                    _logger.LogError("Release: no strategy");
                    return BadRequest("no strategy");  // TODO: this is a terrible way of handling it; await Ronan
                }

                channel.ReevaluateActiveRevision();
                await _unitOfWork.SaveChanges();
                await _channelsToReschedule.Enqueue(new ChannelReference(application.Id, channel.Id), CancellationToken.None);

                _logger.LogInformation($"Release: application {form.Id} channel {channel.Id} now at revision {channel.ActiveRevision.RevisionNumber}");
                _logger.LogInformation($"Release: serving on port {channel.PortID + Channel.EphemeralPortRange}");
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }

        public IActionResult RegisterRevision(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            var a = _unitOfWork.Applications.GetApplicationById(id);
            var vm = new AppRegisterRevisionForm
            {
                Id = a.Id,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterRevision(Guid id, AppRegisterRevisionForm form)
        {
            TraceMethodEntry(WithArgs(id, form));

            if (id != form.Id)
            {
                LogIdMismatch("application", id, form.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var application = _unitOfWork.Applications.GetApplicationById(id);

                application.Revisions.Add(new Revision { RevisionNumber = form.RevisionNumber });
                var changedChannels = application.ReevaluateActiveRevisions();
                await _unitOfWork.SaveChanges();

                // TODO: deduplicate with RevisionController
                var queueRescheduleTasks = changedChannels.Select(channel =>
                    _channelsToReschedule.Enqueue(new ChannelReference(application.Id, channel.Id), CancellationToken.None)
                );

                try
                {
                    await Task.WhenAll(queueRescheduleTasks);
                }
                catch (Exception e)
                {
                    _logger.LogError($"RegisterRevision: failed to queue channel rescheduling for one or more of {String.Join(",", changedChannels.Select(c => c.Name))}: {e}");
                    throw;
                }

                _logger.LogInformation($"RegisterRevision: application {form.Id} registered {form.RevisionNumber}");
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }
    }
}
