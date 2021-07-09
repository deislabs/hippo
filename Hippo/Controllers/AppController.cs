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
                    StorageId = form.StorageId,
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
                StorageId = a.StorageId,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,StorageId")] AppEditForm form)
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

                    var storageIdChanged = (a.StorageId != form.StorageId);

                    a.Name = form.Name;
                    a.StorageId = form.StorageId;

                    IReadOnlyList<Channel> changedChannels = new List<Channel>();

                    if (storageIdChanged)
                    {
                        a.Revisions.Clear();
                        foreach (var channel in a.Channels)
                        {
                            channel.ActiveRevision = null;
                        }
                        changedChannels = new List<Channel>(a.Channels);
                    }

                    _unitOfWork.Applications.Update(a);
                    await _unitOfWork.SaveChanges();

                    if (storageIdChanged)
                    {
                        await QueueChangedChannelNotifications(a, changedChannels);
                    }
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

        public IActionResult NewChannel(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            var a = _unitOfWork.Applications.GetApplicationById(id);
            var vm = new AppNewChannelForm
            {
                Id = a.Id,
                RevisionSelectionStrategies = Converters.EnumValuesAsSelectList<ChannelRevisionSelectionStrategy>(),
                Revisions = a.Revisions.AsSelectList(r => r.RevisionNumber),
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewChannel(Guid id, AppNewChannelForm form)
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

                if (application == null)
                {
                    LogIfNotFound(application, id);
                    return NotFound();
                }

                var channel = new Channel
                {
                    Application = application,
                    Name = form.ChannelName,
                };

                // TODO: in memory seems not to wire up FKs
                application.Channels.Add(channel);

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

                var environmentVariables = ParseEnvironmentVariables(form.EnvironmentVariables).ToList();

                channel.Configuration = new Configuration
                {
                    EnvironmentVariables = environmentVariables,
                };
                channel.Domain = new Domain { Name = form.Domain };

                // TODO: not sure if this is needed
                foreach (var ev in environmentVariables)
                {
                    ev.Configuration = channel.Configuration;
                }

                await _unitOfWork.Channels.AddNew(channel);
                await _unitOfWork.EventLog.ChannelRevisionChanged(EventOrigin.UI, channel, "(none)", "channel created");
                await _unitOfWork.SaveChanges();

                await _channelsToReschedule.Enqueue(new ChannelReference(application.Id, channel.Id), CancellationToken.None);

                _logger.LogInformation($"NewChannel: application {form.Id} channel {channel.Id} now at revision {channel.ActiveRevision.RevisionNumber}");
                _logger.LogInformation($"NewChannel: serving on port {channel.PortID + Channel.EphemeralPortRange}");
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }

        private static IEnumerable<EnvironmentVariable> ParseEnvironmentVariables(string text)
        {
            // TODO: assumes validation in web form - should not assume this
            if (string.IsNullOrWhiteSpace(text))
            {
                return Enumerable.Empty<EnvironmentVariable>();
            }

            return text.Split('\n', ';')
                       .Select(e => e.Trim())
                       .Select(e => e.Split('='))
                       .Select(bits => new EnvironmentVariable { Key = bits[0], Value = bits[1] });
        }

        public IActionResult EditChannel(Guid id)
        {
            TraceMethodEntry(WithArgs(id));

            var channel = _unitOfWork.Channels.GetChannelById(id);
            var application = _unitOfWork.Applications.GetApplicationById(channel.Application.Id);  // To get the revisions
            var vm = new AppEditChannelForm
            {
                ChannelId = channel.Id,
                ApplicationId = application.Id,
                ChannelName = channel.Name,
                RevisionSelectionStrategies = Converters.EnumValuesAsSelectList<ChannelRevisionSelectionStrategy>(),
                SelectedRevisionSelectionStrategy = Enum.GetName(channel.RevisionSelectionStrategy),
                Revisions = application.Revisions.AsSelectList(r => r.RevisionNumber),
                EnvironmentVariables = string.Join('\n', channel.GetEnvironmentVariables().Select(e => $"{e.Key}={e.Value}")),
                Domain = channel.Domain?.Name,
            };
            if (channel.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision)
            {
                vm.SelectedRevisionNumber = channel.SpecifiedRevision?.RevisionNumber;
            }
            else if (channel.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule)
            {
                vm.SelectedRevisionRule = channel.RangeRule;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // NOTE: the arg has to be called id not channelId, and yes it is confusing
        public async Task<IActionResult> EditChannel(Guid id, AppEditChannelForm form)
        {
            TraceMethodEntry(WithArgs(id, form));

            if (id != form.ChannelId)
            {
                LogIdMismatch("channel", id, form.ChannelId);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var channel = _unitOfWork.Channels.GetChannelById(form.ChannelId);
                var application = _unitOfWork.Applications.GetApplicationById(form.ApplicationId);

                if (application == null || channel == null)
                {
                    LogIfNotFound(application, form.ApplicationId);
                    LogIfNotFound(channel, form.ChannelId);
                    return NotFound();
                }

                if (application.Id != channel.Application.Id)
                {
                    LogIdMismatch("application", channel.Application.Id, form.ApplicationId);
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
                        _logger.LogError("EditChannel: empty rule");
                        return BadRequest("rule was empty");  // TODO: this is a terrible way of handling it; await Ronan
                    }
                    channel.RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule;
                    channel.RangeRule = rule;
                }
                else
                {
                    _logger.LogError("EditChannel: no strategy");
                    return BadRequest("no strategy");  // TODO: this is a terrible way of handling it; await Ronan
                }

                var revChange = channel.ReevaluateActiveRevision();

                // TODO: SO MUCH DEDUPLICATION

                // TODO: should probably only update the entities if stuff changes, otherwise
                // we will leak many identical rows into the database
                var environmentVariables = ParseEnvironmentVariables(form.EnvironmentVariables).ToList();

                channel.Configuration = new Configuration
                {
                    EnvironmentVariables = environmentVariables,
                };
                channel.Domain = new Domain { Name = form.Domain };

                // TODO: not sure if this is needed
                foreach (var ev in environmentVariables)
                {
                    ev.Configuration = channel.Configuration;
                }

                if (revChange != null)
                {
                    await _unitOfWork.EventLog.ChannelRevisionChanged(EventOrigin.UI, channel, revChange.ChangedFrom, "channel reconfigured");
                }

                await _unitOfWork.SaveChanges();
                await _channelsToReschedule.Enqueue(new ChannelReference(application.Id, channel.Id), CancellationToken.None);

                _logger.LogInformation($"EditChannel: application {form.ApplicationId} channel {channel.Id} now at revision {channel.ActiveRevision?.RevisionNumber ?? "[none]"}");
                _logger.LogInformation($"EditChannel: serving on port {channel.PortID + Channel.EphemeralPortRange}");
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

                var changes = application.ReevaluateActiveRevisions();
                foreach (var change in changes)
                {
                    await _unitOfWork.EventLog.ChannelRevisionChanged(EventOrigin.UI, change.Channel, change.ChangedFrom, "revision registered");
                }
                await _unitOfWork.SaveChanges();

                await QueueChangedChannelNotifications(application, changes);

                _logger.LogInformation($"RegisterRevision: application {form.Id} registered {form.RevisionNumber}");
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }

        private async Task QueueChangedChannelNotifications(Application application, IReadOnlyList<ActiveRevisionChange> changes) =>
            await QueueChangedChannelNotifications(application, changes.Select(c => c.Channel));

        private async Task QueueChangedChannelNotifications(Application application, IEnumerable<Channel> changedChannels)
        {
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
        }
    }
}
