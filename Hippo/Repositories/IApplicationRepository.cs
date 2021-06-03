using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Repositories
{
    public interface IApplicationRepository
    {
        IEnumerable<Application> ListApplications();
        Application GetApplicationById(Guid id);
        bool ApplicationExistsById(Guid id);
        Task AddNew(Application application);
        void Update(Application application);
        void DeleteApplicationById(Guid id);
    }
}