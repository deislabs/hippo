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
        Task Update(Application application);
        Task DeleteApplicationById(Guid id);
    }
}