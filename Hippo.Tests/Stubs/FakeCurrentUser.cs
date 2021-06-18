using Hippo.Controllers;
using Hippo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Hippo.Tests.Schedulers;
using Hippo.Repositories;
using Hippo.Tests.Stubs;

namespace Hippo.Tests.Stubs
{
    class FakeCurrentUser: ICurrentUser
    {
        private readonly string _name;

        public FakeCurrentUser(string name)
        {
            _name = name;
        }

        public string Name() => _name;
    }
}