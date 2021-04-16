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

        // GET: apps
        public IActionResult Index()
        {
            return View(repository.SelectAllByUser(User.Identity.Name));
        }

        // GET: apps/details/2562dbe3-0317-4895-9536-c0fad46de437
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

        // POST: apps/new
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: apps/edit/2562dbe3-0317-4895-9536-c0fad46de437
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

        // POST: apps/edit/2562dbe3-0317-4895-9536-c0fad46de437
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: apps/delete/2562dbe3-0317-4895-9536-c0fad46de437
        public IActionResult Delete(Guid id)
        {
            var a = repository.SelectByUserAndId(User.Identity.Name, id);
            return View(a);
        }

        // POST: apps/delete/2562dbe3-0317-4895-9536-c0fad46de437
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
