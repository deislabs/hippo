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

namespace Hippo.Controllers
{
    [Authorize]
    public class AppController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Account> _userManager;
        private readonly IJobScheduler _scheduler;

        public AppController(IUnitOfWork unitOfWork, UserManager<Account> userManager, IJobScheduler scheduler, ILogger<AppController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this._scheduler = scheduler;
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
                Id = a.Id
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(Guid id, AppReleaseForm form)
        {
            // TODO: this method is now a bit ill-named.  It is really specifically
            // about updating a specified-revision channel to a new revision.
            TraceMethodEntry(WithArgs(id, form));

            if (id != form.Id)
            {
                LogIdMismatch("application", id, form.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var application = _unitOfWork.Applications.GetApplicationById(id);
                var channel = _unitOfWork.Channels.GetChannelByName(application, form.ChannelName);
                var revision = _unitOfWork.Revisions.GetRevisionByNumber(application, form.Revision);

                if (application != null && channel != null && revision != null)
                {
                    _scheduler.Stop(channel);
                    channel.SpecifiedRevision = revision;
                    channel.ActiveRevision = revision;
                    await _unitOfWork.SaveChanges();
                    _scheduler.Start(channel);
                    _logger.LogInformation($"Release: application {form.Id} channel {channel.Id} revision {form.Revision}: succeeded");
                    _logger.LogInformation($"Release: serving on port {channel.PortID + Channel.EphemeralPortRange}");
                    return RedirectToAction(nameof(Index));
                }

                LogIfNotFound(application, id);
                LogIfNotFound(channel, form.ChannelName);
                LogIfNotFound(revision, form.Revision);

                return NotFound();
            }
            return View(form);
        }
    }
}
