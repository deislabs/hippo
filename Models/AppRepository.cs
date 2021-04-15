using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippo.Models
{
    public interface IAppRepository
    {
        IEnumerable<App> SelectAll();
        App SelectById(Guid id);
        IEnumerable<App> SelectByOwner(string username);
        void Insert(string name);
        void Update(App a);
        App Delete(Guid id);
        void Save();
    }

    public class AppRepository : IAppRepository
    {
        private readonly DataContext context;

        public AppRepository(DataContext context)
        {
            this.context = context;
        }
        public void Insert(string name)
        {
            var app = new App
            {
                Name = name,
            };
            context.Add(app);
        }
        public IEnumerable<App> SelectAll() => context.Applications.OrderBy(a=>a.Name);
        public App SelectById(Guid id) => context.Applications.Where(a=>a.Id == id).Single();
        public IEnumerable<App> SelectByOwner(string username) => context.Applications.Where(a=>a.Owner.UserName==username).OrderBy(a=>a.Name);
        public App Delete(Guid id)
        {
            var app = SelectById(id);
            context.Remove(app);
            return app;
        }
        public void Save() => context.SaveChanges();
        public void Update(App a) => context.Update(a);
    }
}
