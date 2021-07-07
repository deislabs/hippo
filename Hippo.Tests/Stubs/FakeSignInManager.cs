using System;
using System.Threading.Tasks;
using Hippo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Hippo.Tests.Stubs
{
    public class FakeSignInManager : SignInManager<Account>
    {
        public FakeSignInManager()
                : base(new FakeUserManager(),
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IUserClaimsPrincipalFactory<Account>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<ILogger<SignInManager<Account>>>().Object,
                    new Mock<IAuthenticationSchemeProvider>().Object,
                    new Mock<IUserConfirmation<Account>>().Object)
        { }

        public FakeSignInManager(FakeUserManager userManager)
                : base(userManager,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IUserClaimsPrincipalFactory<Account>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<ILogger<SignInManager<Account>>>().Object,
                    new Mock<IAuthenticationSchemeProvider>().Object,
                    new Mock<IUserConfirmation<Account>>().Object)
        { }
    }

    public class FakeUserManager : UserManager<Account>
    {
        private readonly DataContext _context;

        public FakeUserManager()
            : base(new Mock<IUserStore<Account>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<Account>>().Object,
              System.Array.Empty<IUserValidator<Account>>(),
              System.Array.Empty<IPasswordValidator<Account>>(),
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<Account>>>().Object)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Hippo")
                .Options;
            _context = new InMemoryDataContext();
        }

        public FakeUserManager(DataContext context)
            : base(new Mock<IUserStore<Account>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<Account>>().Object,
              System.Array.Empty<IUserValidator<Account>>(),
              System.Array.Empty<IPasswordValidator<Account>>(),
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<Account>>>().Object)
        {
            _context = context;
        }

        public override Task<IdentityResult> CreateAsync(Account user, string password)
        {
            _context.Accounts.Add(user);
            _context.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(Account user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(Account user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

    }

}
