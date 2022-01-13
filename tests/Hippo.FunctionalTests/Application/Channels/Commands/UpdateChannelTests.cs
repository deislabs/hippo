using System;
using System.Threading.Tasks;
using Hippo.Application.Apps.Commands;
using Hippo.Application.Channels.Commands;
using Hippo.Application.Common.Exceptions;
using Hippo.Core.Enums;
using Xunit;

namespace Hippo.FunctionalTests.Application.Channels.Commands;

public class UpdateChannelTests : TestBase
{
    [Theory]
    [InlineData("myapp!example!com")]
    public async Task ShouldValidateDomain(string domain)
    {
        var appId = await SendAsync(new CreateAppCommand
        {
            Name = "updatechanneltests",
            StorageId = "updatechanneltests"
        });

        var createChannelCommand = new CreateChannelCommand
        {
            Name = "testing",
            AppId = appId,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            ActiveRevision = null
        };

        var channelId = await SendAsync(createChannelCommand);

        var command = new UpdateChannelCommand
        {
            Id = channelId,
            Domain = domain,
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
