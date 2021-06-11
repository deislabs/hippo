using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

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
            _context.Applications
                    .Where(application=>application.Owner.UserName == _owner.Name())
                    .Include(a => a.Channels)
                    .Include(a => a.Revisions);

        public IEnumerable<Application> ListApplicationsForAllUsers() =>
            _context.Applications
                    .Include(a => a.Channels)
                    .Include(a => a.Revisions);

        public IEnumerable<Application> ListApplicationsByStorageId(string storageId) =>
            _context.Applications
                    .Where(application => application.StorageId == storageId && application.Owner.UserName == _owner.Name())
                    .Include(a => a.Channels)
                    .Include(a => a.Revisions);

        public Application GetApplicationById(Guid id) =>
            _context.Applications
                    .Where(application => application.Id == id && application.Owner.UserName == _owner.Name())
                    .Include(a => a.Channels)
                    .Include(a => a.Revisions)
                    .SingleOrDefault();

        public bool ApplicationExistsById(Guid id) =>
            _context.Applications.Find(id) != null;

        public async Task AddNew(Application application)
        {
            await _context.Applications.AddAsync(application);
        }

        public void Update(Application application)
        {
            _context.Applications.Update(application);
        }

        public void DeleteApplicationById(Guid id)
        {
            var a = GetApplicationById(id);
            _context.Applications.Remove(a);
        }
    }
}