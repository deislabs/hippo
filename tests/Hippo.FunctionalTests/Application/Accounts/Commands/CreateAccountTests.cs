using System.Threading.Tasks;
using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;

namespace Hippo.FunctionalTests.Application.Accounts.Commands;

public class CreateAccountTests : TestBase
{
    [Fact]
    public async Task ShouldRequireUniqueUserName()
    {
        var command = new CreateAccountCommand
        {
            UserName = "bob",
            Password = "Passw0rd!",
            PasswordConfirm = "Passw0rd!"
        };

        await SendAsync(command);

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Theory]
    [InlineData("bacongobbler1", "Passw0rd!")]
    [InlineData("bacongobbler2", "Password!")]
    [InlineData("bacongobbler3", "password")]
    [InlineData("bacongobbler4", "Password")]
    [InlineData("bacongobbler5", "password!")]
    [InlineData("bacongobbler7", "a")]
    [InlineData("bacongobbler8", "A")]
    [InlineData("bacongobbler9", "1")]
    [InlineData("bacongobbler10", "!")]
    public void ShouldCreateAccount(string userName, string password)
    {
        var command = new CreateAccountCommand
        {
            UserName = userName,
            Password = password,
            PasswordConfirm = password
        };

        Assert.True(SendAsync(command).IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData("", "Passw0rd!", "Passw0rd!")]
    [InlineData("bacongobbler", "", "Passw0rd!")]
    [InlineData("bacongobbler", "Passw0rd!", "")]
    [InlineData("", "", "Passw0rd!")]
    [InlineData("bacongobbler", "", "")]
    [InlineData("", "Passw0rd!", "")]
    [InlineData("", "", "")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Passw0rd!", "Passw0rd!")]
    [InlineData("!@#$%^&*(){}[]<>\\|'\";:,./?=+", "Passw0rd!", "Passw0rd!")]
    [InlineData("Bobby Tables", "Passw0rd!", "Passw0rd!")]
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
