using System;
using System.Collections.Generic;
using System.Linq;

namespace Hippo.Models
{
    public interface IAppRepository
    {
        IEnumerable<Application> SelectAll();
        IEnumerable<Application> SelectAllByUser(string username);
        Application SelectById(Guid id);
        Application SelectByUserAndId(string username, Guid id);
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

        public Application SelectByUserAndId(string username, Guid id) => context.Applications.Where(a=>a.Id==id && a.Owner.UserName==username).Single();
    }
}
