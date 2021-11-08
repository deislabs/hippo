using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Hippo.Core.Models;
using Hippo.FunctionalTests.Schedulers;
using Hippo.FunctionalTests.Stubs;
using Hippo.Infrastructure.Data;
using Hippo.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Hippo.FunctionalTests.Stubs
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
