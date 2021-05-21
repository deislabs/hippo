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
            return View(_unitOfWork.Applications.ListApplications());
        }

        public IActionResult Details(Guid id)
        {
            var a = _unitOfWork.Applications.GetApplicationById(id);
            if (a == null)
            {
                return NotFound();
            }

            return View(a);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(AppNewForm form)
        {
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
            var a = _unitOfWork.Applications.GetApplicationById(id);
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
            if (id != form.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var a = _unitOfWork.Applications.GetApplicationById(id);
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        public IActionResult Delete(Guid id)
        {
            var a = _unitOfWork.Applications.GetApplicationById(id);
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
            _unitOfWork.Applications.DeleteApplicationById(id);
            await _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Release(Guid id)
        {
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
            _logger.LogInformation($"Release: form application {form.Id} revision {form.Revision}: starting");

            if (id != form.Id)
            {
                _logger.LogWarning($"Release: application ID {form.Id} did not match expected ID {id}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var application = _unitOfWork.Applications.GetApplicationById(id);
                var channel = _unitOfWork.Channels.GetChannelByName(application, form.ChannelName);
                var release = _unitOfWork.Releases.GetReleaseByRevision(application, form.Revision);

                if (application != null && channel != null && release != null)
                {
                    _scheduler.Stop(channel);
                    channel.Release = release;
                    await _unitOfWork.SaveChanges();
                    _scheduler.Start(channel);
                    _logger.LogInformation($"Release: form application {form.Id} revision {form.Revision}: succeeded");
                    return RedirectToAction(nameof(Index));
                }

                LogIfNotFound(application, id);
                LogIfNotFound(channel, form.ChannelName);
                LogIfNotFound(release, form.Revision);

                return NotFound();
            }
            return View(form);
        }
    }
}
