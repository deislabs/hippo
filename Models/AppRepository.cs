using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Models
{
    public interface IAppRepository
    {
        IEnumerable<App> SelectAll();

        IEnumerable<App> SelectAllByUser(string username);
        App SelectById(Guid id);

        App SelectByUserAndId(string username, Guid id);
        void Insert(App a);
        void Update(App a);
        void Delete(App a);
        void Save();
    }

    public class AppRepository : IAppRepository
    {
        private readonly DataContext context;

        public AppRepository(DataContext context)
        {
            this.context = context;
        }
        public void Insert(App a) => context.Add(a);
        public IEnumerable<App> SelectAll() => context.Applications.OrderBy(a=>a.Name);
        public App SelectById(Guid id) => context.Applications.Where(a=>a.Id==id).Single();
        public void Delete(App a)
        {
            context.Remove(a);
        }
        public void Save() => context.SaveChanges();
        public void Update(App a) => context.Update(a);

        public IEnumerable<App> SelectAllByUser(string username) => context.Applications.Where(a=>a.Owner.UserName==username).OrderBy(a=>a.Name);

        public App SelectByUserAndId(string username, Guid id) => context.Applications.Where(a=>a.Id==id && a.Owner.UserName==username).Single();
    }
}
