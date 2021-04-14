using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hippo.Models;
using Hippo.ViewModels;

namespace Hippo.Controllers
{
    public class AppsController : Controller
    {
        private readonly IDataRepository repository;

        public AppsController(IDataRepository repository)
        {
            this.repository = repository;
        }

        // GET: apps
        public IActionResult Index()
        {
            return View(repository.GetAllApps());
        }

        // GET: apps/details/2562dbe3-0317-4895-9536-c0fad46de437
        public IActionResult Details(Guid id)
        {
            var application = repository.GetAppById(id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
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
        public IActionResult New([Bind("Id,Name")] App application)
        {
            if (ModelState.IsValid)
            {
                repository.AddApp(application.Name);
                repository.SaveAll();
                return RedirectToAction(nameof(Index));
            }
            return View(application);
        }

        // GET: apps/edit/2562dbe3-0317-4895-9536-c0fad46de437
        public IActionResult Edit(Guid id)
        {
            var application = repository.GetAppById(id);
            if (application == null)
            {
                return NotFound();
            }

            AppEditForm vm = new AppEditForm
            {
                Id = application.Id,
                Name = application.Name,
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
                    var application = repository.GetAppById(id);
                    application.Name = form.Name;
                    repository.Update(application);
                    repository.SaveAll();
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
            return View(repository.GetAppById(id));
        }

        // POST: apps/delete/2562dbe3-0317-4895-9536-c0fad46de437
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            repository.RemoveAppById(id);
            repository.SaveAll();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(Guid id)
        {
            return repository.GetAppById(id) != null;
        }
    }
}
