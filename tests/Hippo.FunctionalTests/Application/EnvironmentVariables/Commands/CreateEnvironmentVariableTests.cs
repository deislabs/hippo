using System.Threading.Tasks;
using Hippo.Application.EnvironmentVariables.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;

namespace Hippo.FunctionalTests.Application.EnvironmentVariables.Commands;

public class CreateEnvironmentVariableTests : TestBase
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateEnvironmentVariableCommand()));
    }

    [Theory]
    [InlineData("FOO", "bar")]
    [InlineData("FOO", "")]
    public async Task ShouldCreate(string key, string value)
    {
        var command = new CreateEnvironmentVariableCommand
        {
            Key = key,
            Value = value
        };

        await SendAsync(command);
    }

    [Theory]
    [InlineData("", "bar")]
    public async Task ShouldNotCreate(string key, string value)
    {
        var command = new CreateEnvironmentVariableCommand
        {
            Key = key,
            Value = value
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
