using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Hippo.Repositories
{
    public class DbApplicationRepository: IApplicationRepository
    {
        private readonly DataContext _context;
        private readonly ICurrentUser _owner;

        public DbApplicationRepository(DataContext context, ICurrentUser owner)
        {
            _context = context;
            _owner = owner;
        }

        public IEnumerable<Application> ListApplications() =>
            _context.Applications.Where(application=>application.Owner.UserName == _owner.Name());

        public Application GetApplicationById(Guid id) =>
            _context.Applications.Where(application => application.Id == id && application.Owner.UserName == _owner.Name()).SingleOrDefault();

        public bool ApplicationExistsById(Guid id) =>
            _context.Applications.Find(id) != null;

        public async Task AddNew(Application application)
        {
                await _context.Applications.AddAsync(application);
                await _context.SaveChangesAsync();
        }

        public async Task Update(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteApplicationById(Guid id)
        {
            var a = GetApplicationById(id);
            _context.Applications.Remove(a);
            await _context.SaveChangesAsync();

        }
    }
}