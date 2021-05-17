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

namespace Hippo.Controllers
{
    [Authorize]
    public class AppController : Controller
    {
        private readonly DataContext context;
        private readonly UserManager<Account> userManager;
        private readonly IWebHostEnvironment environment;

        public AppController(DataContext context, UserManager<Account> userManager, IWebHostEnvironment environment)
        {
            this.context = context;
            this.userManager = userManager;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            return View(context.Applications.Where(application=>application.Owner.UserName==User.Identity.Name));
        }

        public IActionResult Details(Guid id)
        {
            var a = context.Applications.Where(application=>application.Id==id && application.Owner.UserName==User.Identity.Name).SingleOrDefault();
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
                context.Applications.Add(new Application
                {
                    Name = form.Name,
                    Owner = await userManager.FindByNameAsync(User.Identity.Name),
                });
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        public IActionResult Edit(Guid id)
        {
            var a = context.Applications.Where(application=>application.Id==id && application.Owner.UserName==User.Identity.Name).SingleOrDefault();
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
        public IActionResult Edit(Guid id, [Bind("Id,Name")] AppEditForm form)
        {
            if (id != form.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var a = context.Applications.Where(application=>application.Id==id && application.Owner.UserName==User.Identity.Name).SingleOrDefault();
                    if (a == null)
                    {
                        return NotFound();
                    }

                    a.Name = form.Name;
                    context.Applications.Update(a);
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(form.Id))
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
            var a = context.Applications.Where(application=>application.Id==id && application.Owner.UserName==User.Identity.Name).SingleOrDefault();
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var a = context.Applications.Where(application=>application.Id==id && application.Owner.UserName==User.Identity.Name).SingleOrDefault();
            context.Applications.Remove(a);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Release(Guid id)
        {
            var a = context.Applications.Where(application=>application.Id==id && application.Owner.UserName==User.Identity.Name).SingleOrDefault();
            var vm = new AppReleaseForm
            {
                Id = a.Id
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Release(Guid id, AppReleaseForm form)
        {
            if (id != form.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var application = context.Applications
                    .Where(application=>application.Id==id && application.Owner.UserName==User.Identity.Name)
                    .SingleOrDefault();
                var channel = context.Channels
                    .Where(c => c.Application == application && c.Name == form.ChannelName)
                    .Include(c => c.Application)
                    .Include(c => c.Configuration)
                        .ThenInclude(c => c.EnvironmentVariables)
                    .Include(c => c.Domain)
                    .Include(c => c.Release)
                    .SingleOrDefault();
                var release = context.Releases
                    .Where(r => r.Application == application && r.Revision == form.Revision)
                    .SingleOrDefault();

                if (application != null && channel != null && release != null)
                {
                    channel.Stop();
                    channel.Release = release;
                    context.SaveChanges();
                    channel.Start();
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(form);
        }

        private bool ApplicationExists(Guid id)
        {
            return context.Applications.Find(id) != null;
        }
    }
}
