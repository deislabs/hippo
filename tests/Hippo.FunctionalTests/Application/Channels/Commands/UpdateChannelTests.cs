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
            Name = "ShouldValidateDomain",
            StorageId = "ShouldValidateDomain"
        });

        var createChannelCommand = new CreateChannelCommand
        {
            Name = "ShouldValidateDomain",
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

    [Fact]
    public async Task ShouldValidateUniqueName()
    {
        var appId = await SendAsync(new CreateAppCommand
        {
            Name = "ShouldValidateUniqueName",
            StorageId = "ShouldValidateUniqueName"
        });

        var createChannelCommand = new CreateChannelCommand
        {
            Name = "ShouldValidateUniqueName",
            AppId = appId,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            ActiveRevision = null
        };

        var channelId = await SendAsync(createChannelCommand);

        await SendAsync(new CreateChannelCommand
        {
            Name = "ShouldValidateUniqueName2",
            AppId = appId,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            ActiveRevision = null
        });

        var command = new UpdateChannelCommand
        {
            Id = channelId,
            Name = "ShouldValidateUniqueName2"
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
