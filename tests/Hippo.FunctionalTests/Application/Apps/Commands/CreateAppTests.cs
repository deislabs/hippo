using System.Threading.Tasks;
using Hippo.Application.Apps.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;

namespace Hippo.FunctionalTests.Application.Apps.Commands;

public class CreateAccountTests : TestBase
{
    [Fact]
    public async Task ShouldRequireUniqueName()
    {
        var command = new CreateAppCommand
        {
            Name = "thisisauniquename",
            StorageId = "foo"
        };

        await SendAsync(command);

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Theory]
    [InlineData("bar", "bar")]
    [InlineData("name-with-hyphens", "bar")]
    [InlineData("name_with_underscores", "bar")]
    [InlineData("storage-id-with-hyphens", "my-app")]
    [InlineData("storage-id-with-underscores", "my_app")]
    [InlineData("storage-id-with-slash", "bacongobbler/myapp")]
    [InlineData("storage-id-with-dot", "github.com/bacongobbler/myapp")]
    public void ShouldCreateApp(string name, string storageId)
    {
        var command = new CreateAppCommand
        {
            Name = name,
            StorageId = storageId
        };

        Assert.True(SendAsync(command).IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("foo", "")]
    [InlineData("", "foo")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "foo")]
    [InlineData("foo", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("!@#$%^&*(){}[]<>\\|'\";:,/?=+", "foo")]
    public async Task ShouldNotCreateApp(string name, string storageId)
    {
        var command = new CreateAppCommand
        {
            Name = name,
            StorageId = storageId
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
