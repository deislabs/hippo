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
            Password = "Passw0rd!"
        };

        await SendAsync(command);

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Theory]
    [InlineData("bacongobbler1", "Passw0rd!")]
    [InlineData("bacongobbler2", "Password!")]
    [InlineData("bacongobbler3", "password!")]
    [InlineData("bacongobbler4", "password")]
    [InlineData("bacongobbler5", "Password")]
    [InlineData("bacongobbler6", "123456")]
    public void ShouldCreateAccount(string userName, string password)
    {
        var command = new CreateAccountCommand
        {
            UserName = userName,
            Password = password
        };

        Assert.True(SendAsync(command).IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("bacongobbler", "")]
    [InlineData("", "Passw0rd!")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Passw0rd!")]
    [InlineData("!@#$%^&*(){}[]<>\\|'\";:,./?=+", "Passw0rd!")]
    [InlineData("bacongobbler", "a")]
    [InlineData("bacongobbler", "12345")]
    public async Task ShouldNotCreateAccount(string userName, string password)
    {
        var command = new CreateAccountCommand
        {
            UserName = userName,
            Password = password
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
