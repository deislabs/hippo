using System;
using System.Collections.Generic;
using System.Linq;
using Hippo.Models;

namespace Hippo.Tests.Models
{
    internal class FakeAppRepository: IAppRepository
    {
        private readonly List<Application> Applications;

        public FakeAppRepository()
        {
            Applications = new List<Application>
            {
                new Application{
                    Name = "one",
                    Owner = new Account
                    {
                        UserName = "admin"
                    }
                }
            };
        }

        public void AddRelease(Application a, Release r)
        {
            throw new NotImplementedException();
        }
        
        public void Delete(Application a)
        {
            throw new NotImplementedException();
        }

        public void Insert(Application a)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Application> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Application> SelectAllByUser(string username)
        {
            return Applications.Where(a => a.Owner.UserName == username);
        }

        public Application SelectById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Application SelectByUserAndId(string username, Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Application a)
        {
            Applications.Add(a);
        }
    }
}
