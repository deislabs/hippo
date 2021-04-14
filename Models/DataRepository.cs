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

        public IEnumerable<App> SelectAll()
        {
            var query = from a in context.Applications
                        orderby a.Name
                        select a;
            return query.ToList();
        }

        public App SelectById(Guid id)
        {
            var query = from a in context.Applications
                        where a.Id == id
                        select a;
            return query.Single();
        }

        public IEnumerable<App> SelectByOwner(string username)
        {
            var query = from a in context.Applications
                        where a.Owner.UserName == username
                        orderby a.Name
                        select a;
            return query.ToList();
        }

        public App Delete(Guid id)
        {
            var app = SelectById(id);
            context.Remove(app);
            return app;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(App a)
        {
            context.Update(a);
        }
    }
}
