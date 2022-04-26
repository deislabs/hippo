using System.Threading.Tasks;
using Hippo.Application.Revisions.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;
using Hippo.Application.Apps.Commands;

namespace Hippo.FunctionalTests.Application.Revisions.Commands;

public class ImportRevisionsTests : TestBase
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateRevisionCommand()));
    }

    [Theory]
    [InlineData("1.0.0")]
    public async Task ShouldCreate(string revisionNumber)
    {
        var appId = await SendAsync(new CreateAppCommand
        {
            Name = "foobar",
            StorageId = "spin-hello-world",
        });

        var command = new ImportRevisionsCommand
        {
            AppId = appId,
        };

        await SendAsync(command);
    }
}
