using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace Hippo.Models
{
    public interface IAppRepository
    {
        IEnumerable<Application> SelectAll();
        IEnumerable<Application> SelectAllByUser(string username);
        Application SelectById(Guid id);
        Application SelectByUserAndId(string username, Guid id);

        void AddRelease(Application a, Release r);
        void Insert(Application a);
        void Update(Application a);
        void Delete(Application a);
        void Save();
    }

    public class AppRepository : IAppRepository
    {
        private readonly DataContext context;

        public AppRepository(DataContext context)
        {
            this.context = context;
        }
        public void Insert(Application a) => context.Add(a);
        public IEnumerable<Application> SelectAll() => context.Applications.OrderBy(a=>a.Name);
        public Application SelectById(Guid id) => context.Applications.Where(a=>a.Id==id).Single();
        public void Delete(Application a) => context.Remove(a);
        public void Save() => context.SaveChanges();
        public void Update(Application a) => context.Update(a);

        public IEnumerable<Application> SelectAllByUser(string username) => context.Applications.Where(a=>a.Owner.UserName==username).OrderBy(a=>a.Name);

        public Application SelectByUserAndId(string username, Guid id) => context.Applications.Where(a=>a.Id==id && (a.Owner.UserName==username || a.Collaborators.Exists(match=>match.UserName==username))).Single();

        public void AddRelease(Application a, Release release)
        {
            a.Releases.Add(release);
            foreach (Channel c in a.Channels)
            {
                if (c.AutoDeploy)
                {
                    var filter = VersionRange.Parse(c.VersionRange);
                    // find the latest version that satisfies the filter.
                    foreach (Release r in a.Releases.OrderBy(r => r.Revision))
                    {
                        if (filter.Satisfies(NuGetVersion.Parse(r.Revision)))
                        {
                            c.UnPublish();
                            c.Release = r;
                            c.Publish();
                        }
                    }
                }
            }
        }
    }
}
