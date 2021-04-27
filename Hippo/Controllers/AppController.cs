using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hippo.Models;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace Hippo.Controllers
{
    [Authorize]
    public class AppController : Controller
    {
        private readonly IAppRepository repository;
        private readonly UserManager<Account> userManager;
        private readonly IWebHostEnvironment environment;

        public AppController(IAppRepository repository, UserManager<Account> userManager, IWebHostEnvironment environment)
        {
            this.repository = repository;
            this.userManager = userManager;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            return View(repository.SelectAllByUser(User.Identity.Name));
        }

        public IActionResult Details(Guid id)
        {
            var a = repository.SelectByUserAndId(User.Identity.Name, id);
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
                repository.Insert(new App
                {
                    Name = form.Name,
                    Owner = await userManager.FindByNameAsync(User.Identity.Name),
                });
                repository.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        public IActionResult Edit(Guid id)
        {
            var a = repository.SelectByUserAndId(User.Identity.Name, id);
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
                    var a = repository.SelectByUserAndId(User.Identity.Name, id);

                    a.Name = form.Name;
                    repository.Update(a);
                    repository.Save();
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
            var a = repository.SelectByUserAndId(User.Identity.Name, id);
            return View(a);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var a = repository.SelectByUserAndId(User.Identity.Name, id);
            repository.Delete(a);
            repository.Save();
            return RedirectToAction(nameof(Index));
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
                var a = repository.SelectByUserAndId(User.Identity.Name, id);
                a.DeployTo(form.Revision, environment.ContentRootPath);
                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        private bool ApplicationExists(Guid id)
        {
            return repository.SelectByUserAndId(User.Identity.Name, id) != null;
        }
    }
}
