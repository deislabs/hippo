using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Exceptions;
using Hippo.Infrastructure.Identity;
using Xunit;

namespace Hippo.FunctionalTests.Application.Accounts.Commands;

public class CreateAccountTests : TestBase
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateAccountCommand()));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateAccountCommand { UserName = "bacongobbler" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateAccountCommand { Password = "bacongobbler" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateAccountCommand { PasswordConfirm = "bacongobbler" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateAccountCommand { UserName = "bacongobbler", Password = "Passw0rd!" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateAccountCommand { UserName = "bacongobbler", PasswordConfirm = "Passw0rd!" }));
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateAccountCommand { Password = "Passw0rd!", PasswordConfirm = "Passw0rd!" }));
    }

    [Fact]
    public async Task ShouldRequireUniqueUserName()
    {
        await SendAsync(new CreateAccountCommand
        {
            UserName = "bacongobbler",
            Password = "Passw0rd!",
            PasswordConfirm = "Passw0rd!"
        });

        var command = new CreateAccountCommand
        {
            UserName = "bacongobbler",
            Password = "Passw0rd!",
            PasswordConfirm = "Passw0rd!"
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Fact]
    public async Task ShouldCreateAccount()
    {
        var command = new CreateAccountCommand
        {
            UserName = "bacongobbler",
            Password = "Passw0rd!",
            PasswordConfirm = "Passw0rd!"
        };

        var userId = await SendAsync(command);
        Guid x;
        Assert.True(Guid.TryParse(userId, out x));
    }
}
