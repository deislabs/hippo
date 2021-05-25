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

namespace Hippo.Controllers
{
    [Authorize]
    public class AppController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<Account> userManager;
        private readonly IJobScheduler scheduler;

        public AppController(IUnitOfWork unitOfWork, UserManager<Account> userManager, IJobScheduler scheduler)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.scheduler = scheduler;
        }

        public IActionResult Index()
        {
            return View(unitOfWork.Applications.ListApplications());
        }

        public IActionResult Details(Guid id)
        {
            var a = unitOfWork.Applications.GetApplicationById(id);
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
                await unitOfWork.Applications.AddNew(new Application
                {
                    Name = form.Name,
                    Owner = await userManager.FindByNameAsync(User.Identity.Name),
                });
                await unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        public IActionResult Edit(Guid id)
        {
            var a = unitOfWork.Applications.GetApplicationById(id);
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
                    var a = unitOfWork.Applications.GetApplicationById(id);
                    if (a == null)
                    {
                        return NotFound();
                    }

                    a.Name = form.Name;
                    unitOfWork.Applications.Update(a);
                    await unitOfWork.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!unitOfWork.Applications.ApplicationExistsById(form.Id))
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
            var a = unitOfWork.Applications.GetApplicationById(id);
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
            unitOfWork.Applications.DeleteApplicationById(id);
            await unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Release(Guid id)
        {
            var a = unitOfWork.Applications.GetApplicationById(id);
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
            if (id != form.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var application = unitOfWork.Applications.GetApplicationById(id);
                var channel = unitOfWork.Channels.GetChannelByName(application, form.ChannelName);
                var release = unitOfWork.Releases.GetReleaseByRevision(application, form.Revision);

                if (application != null && channel != null && release != null)
                {
                    scheduler.Stop(channel);
                    channel.Release = release;
                    await unitOfWork.SaveChanges();
                    scheduler.Start(channel);
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(form);
        }
    }
}
