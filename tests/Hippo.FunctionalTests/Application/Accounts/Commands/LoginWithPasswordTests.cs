using System.Threading.Tasks;
using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;

namespace Hippo.FunctionalTests.Application.Accounts.Commands;

public class LoginWithPasswordTests : TestBase
{
    [Fact]
    public async Task TestLoginWithPasswordFlow()
    {
        var userName = RandomString(10);
        var password = RandomString(10);

        var userId = await SendAsync(new CreateAccountCommand
        {
            UserName = userName
        });

        // password has not been added to the account
        await Assert.ThrowsAsync<LoginFailedException>(
            async () => await SendAsync(
                new LoginWithPasswordCommand
                {
                    UserName = userName,
                    Password = password,
                }
            )
        );

        // add a password to the account
        await SendAsync(new AddPasswordCommand
        {
            UserName = userName,
            Password = password
        });

        // wrong password
        await Assert.ThrowsAsync<LoginFailedException>(
            async () => await SendAsync(
                new LoginWithPasswordCommand
                {
                    UserName = userName,
                    Password = RandomString(10),
                }
            )
        );

        // wrong username
        await Assert.ThrowsAsync<LoginFailedException>(
            async () => await SendAsync(
                new LoginWithPasswordCommand
                {
                    UserName = RandomString(10),
                    Password = password,
                }
            )
        );

        await SendAsync(new LoginWithPasswordCommand
        {
            UserName = userName,
            Password = password
        });
    }
}
