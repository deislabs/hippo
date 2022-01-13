using System.Threading.Tasks;
using Hippo.Application.Revisions.Commands;
using Hippo.Application.Common.Exceptions;
using Xunit;
using Hippo.Application.Apps.Commands;
using Hippo.Core.Entities;
using System;

namespace Hippo.FunctionalTests.Application.Revisions.Commands;

public class CreateRevisionTests : TestBase
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
            Name = "bacongobbler/foo"
        });

        var command = new CreateRevisionCommand
        {
            RevisionNumber = revisionNumber,
            AppId = appId
        };

        await SendAsync(command);
    }

    [Theory]
    [InlineData("")]
    public async Task ShouldNotCreate(string revisionNumber)
    {
        var appId = await SendAsync(new CreateAppCommand
        {
            Name = "bacongobbler/foobar"
        });

        var command = new CreateRevisionCommand
        {
            RevisionNumber = revisionNumber,
            AppId = appId
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
