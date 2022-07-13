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
            UserName = "bob"
        };

        await SendAsync(command);

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Theory]
    [InlineData("bacongobbler")]
    [InlineData("Camel")]
    [InlineData("CamelCamel")]
    [InlineData("IRequireAShrubbery")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public void ShouldCreateAccount(string userName)
    {
        var command = new CreateAccountCommand
        {
            UserName = userName
        };

        Assert.True(SendAsync(command).IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData("")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("!@#$%^&*(){}[]<>\\|'\";:,./?=+")]
    public async Task ShouldNotCreateAccount(string userName)
    {
        var command = new CreateAccountCommand
        {
            UserName = userName
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
