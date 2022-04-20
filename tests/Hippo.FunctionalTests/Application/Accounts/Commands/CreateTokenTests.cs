using System.Threading.Tasks;
using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;

namespace Hippo.FunctionalTests.Application.Accounts.Commands;

public class CreateTokenTests : TestBase
{
    [Fact]
    public async Task ShouldRaiseLoginFailedException()
    {
        var userName = RandomString(10);
        var password = "Passw0rd!";

        await SendAsync(new CreateAccountCommand
        {
            UserName = userName,
            Password = password,
            PasswordConfirm = password
        });

        await Assert.ThrowsAsync<LoginFailedException>(
            async () => await SendAsync(
                new CreateTokenCommand
                {
                    UserName = userName,
                    Password = RandomString(10),
                }
            )
        );
    }
}
