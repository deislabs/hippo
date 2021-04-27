using System;
using System.Collections.Generic;
using System.Linq;
using Hippo.Models;

namespace Hippo.Tests
{
    internal class FakeAppRepository: IAppRepository
    {
        private readonly List<App> Applications;

        public FakeAppRepository()
        {
            Applications = new List<App>
            {
                new App{
                    Name = "one",
                    Owner = new Account
                    {
                        UserName = "admin"
                    }
                }
            };
        }

        public void Delete(App a)
        {
            throw new NotImplementedException();
        }

        public void Insert(App a)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<App> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<App> SelectAllByUser(string username)
        {
            return Applications.Where(a => a.Owner.UserName == username);
        }

        public App SelectById(Guid id)
        {
            throw new NotImplementedException();
        }

        public App SelectByUserAndId(string username, Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(App a)
        {
            Applications.Add(a);
        }
    }
}
