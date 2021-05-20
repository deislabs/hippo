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
        private readonly IApplicationRepository applications;
        private readonly IChannelRepository channels;
        private readonly IReleaseRepository releases;
        private readonly UserManager<Account> userManager;
        private readonly IWebHostEnvironment environment;
        private readonly IJobScheduler scheduler;

        public AppController(IApplicationRepository applications, IChannelRepository channels, IReleaseRepository releases, UserManager<Account> userManager, IWebHostEnvironment environment, IJobScheduler scheduler)
        {
            this.applications = applications;
            this.channels = channels;
            this.releases = releases;
            this.userManager = userManager;
            this.environment = environment;
            this.scheduler = scheduler;
        }

        public IActionResult Index()
        {
            return View(applications.ListApplications());
        }

        public IActionResult Details(Guid id)
        {
            var a = applications.GetApplicationById(id);
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
                await applications.AddNew(new Application
                {
                    Name = form.Name,
                    Owner = await userManager.FindByNameAsync(User.Identity.Name),
                });
                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        public IActionResult Edit(Guid id)
        {
            var a = applications.GetApplicationById(id);
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
                    var a = applications.GetApplicationById(id);
                    if (a == null)
                    {
                        return NotFound();
                    }

                    a.Name = form.Name;
                    await applications.Update(a);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!applications.ApplicationExistsById(form.Id))
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
            var a = applications.GetApplicationById(id);
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
            await applications.DeleteApplicationById(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Release(Guid id)
        {
            var a = applications.GetApplicationById(id);
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
                var application = applications.GetApplicationById(id);
                var channel = channels.GetChannelByName(application, form.ChannelName);
                var release = releases.GetReleaseByRevision(application, form.Revision);

                if (application != null && channel != null && release != null)
                {
                    scheduler.Stop(channel);
                    channel.Release = release;
                    await channels.SaveChanges();
                    scheduler.Start(channel);
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(form);
        }
    }
}
