using System.Threading.Tasks;
using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Exceptions;
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

    [Theory]
    [InlineData("bacongobbler", "Passw0rd!", "Passw0rd!")]
    public void ShouldCreateAccount(string userName, string password, string passwordConfirm)
    {
        var command = new CreateAccountCommand
        {
            UserName = userName,
            Password = password,
            PasswordConfirm = passwordConfirm
        };

        Assert.True(SendAsync(command).IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Passw0rd!", "Passw0rd!")]
    [InlineData("!@#$%^&*(){}[]<>\\|'\";:,./?=+", "Passw0rd!", "Passw0rd!")]
    [InlineData("Bobby Tables", "Passw0rd!", "Passw0rd!")]
    [InlineData("", "Passw0rd!", "Passw0rd!")]
    [InlineData("bacongobbler", "", "")]
    [InlineData("bacongobbler", "", "Passw0rd!")]
    [InlineData("bacongobbler", "Passw0rd!", "")]
    public async Task ShouldNotCreateAccount(string userName, string password, string passwordConfirm)
    {
        var command = new CreateAccountCommand
        {
            UserName = userName,
            Password = password,
            PasswordConfirm = passwordConfirm
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
