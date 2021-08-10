using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Hippo.Controllers;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tests.Schedulers;
using Hippo.Tests.Stubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Hippo.Tests.Stubs
{
    class FakeCurrentUser : ICurrentUser
    {
        private readonly string _name;

        public FakeCurrentUser(string name)
        {
            _name = name;
        }

        public string Name() => _name;
    }
}
