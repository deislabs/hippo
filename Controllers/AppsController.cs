using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hippo.Models;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Controllers
{
    [Authorize]
    public class AppsController : Controller
    {
        private readonly IAppRepository repository;
        private readonly UserManager<Account> userManager;

        public AppsController(IAppRepository repository, UserManager<Account> userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
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

        // GET: apps/new
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

            AppEditForm vm = new AppEditForm
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

        private bool ApplicationExists(Guid id)
        {
            return repository.SelectByUserAndId(User.Identity.Name, id) != null;
        }
    }
}
