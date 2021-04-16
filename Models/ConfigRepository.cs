using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Models
{
    public interface IConfigRepository
    {
        IEnumerable<Config> SelectAll();

        IEnumerable<Config> SelectAllByUser(string username);
        Config SelectById(Guid id);

        Config SelectByUserAndId(string username, Guid id);
        void Insert(Config c);
        void Update(Config c);
        void Delete(Config c);
        void Save();
    }

    public class ConfigRepository : IConfigRepository
    {
        private readonly DataContext context;

        public ConfigRepository(DataContext context)
        {
            this.context = context;
        }

        public void Delete(Config c) => context.Remove(c);

        public void Insert(Config c) => context.Add(c);

        public void Save() => context.SaveChanges();

        public IEnumerable<Config> SelectAll() => context.Configuration;

        public IEnumerable<Config> SelectAllByUser(string username) => context.Configuration.Where(c=>c.App.Owner.UserName==username);

        public Config SelectById(Guid id) => context.Configuration.Where(c=>c.Id==id).Single();

        public Config SelectByUserAndId(string username, Guid id) => context.Configuration.Where(c=>c.Id==id&&c.App.Owner.UserName==username).Single();

        public void Update(Config c) => context.Update(c);
    }
}
