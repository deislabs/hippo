using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippo.Models
{
    public interface IDataRepository
    {
        IEnumerable<App> GetAllApps();
        App GetAppById(Guid id);
        IEnumerable<App> GetAppsByUser(string username);
        void AddApp(string name);
        App RemoveAppById(Guid id);
        void Update<TEntity>(TEntity a);
        bool SaveAll();
    }

    public class DataRepository : IDataRepository
    {
        private readonly DataContext context;

        public DataRepository(DataContext context)
        {
            this.context = context;
        }

        public void AddApp(string name)
        {
            var app = new App
            {
                Name = name,
            };
            context.Add(app);
        }

        public IEnumerable<App> GetAllApps()
        {
            var query = from a in context.Applications
                        orderby a.Name
                        select a;
            return query.ToList();
        }

        public App GetAppById(Guid id)
        {
            var query = from a in context.Applications
                        where a.Id == id
                        select a;
            return query.Single();
        }

        public IEnumerable<App> GetAppsByUser(string username)
        {
            var query = from a in context.Applications
                        where a.Owner.UserName == username
                        orderby a.Name
                        select a;
            return query.ToList();
        }

        public App RemoveAppById(Guid id)
        {
            var app = GetAppById(id);
            context.Remove(app);
            return app;
        }

        public bool SaveAll()
        {
            return context.SaveChanges() > 0;
        }

        public void Update<TEntity>(TEntity a)
        {
            context.Update(a);
        }
    }
}
