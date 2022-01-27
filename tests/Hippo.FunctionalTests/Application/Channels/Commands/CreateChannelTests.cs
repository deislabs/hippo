using System;
using System.Threading.Tasks;
using Hippo.Application.Channels.Commands;
using Hippo.Application.Common.Exceptions;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Xunit;

namespace Hippo.FunctionalTests.Application.Channels.Commands;

public class CreateChannelTests : TestBase
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new CreateChannelCommand()));
    }

    [Fact]
    public async Task ShouldRequireUniqueChannelNameForApp()
    {
        var appId1 = Guid.NewGuid();
        var appId2 = Guid.NewGuid();

        await AddAsync(new App
        {
            Id = appId1,
            Name = RandomString(10),
            StorageId = RandomString(10),
        });

        await AddAsync(new App
        {
            Id = appId2,
            Name = RandomString(10),
            StorageId = RandomString(10),
        });

        await SendAsync(new CreateChannelCommand
        {
            Name = "development",
            AppId = appId1,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*"
        });

        var command = new CreateChannelCommand
        {
            Name = "development",
            AppId = appId1,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*"
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));

        command.AppId = appId2;

        await SendAsync(command);
    }

    [Theory]
    [InlineData("production", ChannelRevisionSelectionStrategy.UseRangeRule, "*", null)]
    [InlineData("staging", ChannelRevisionSelectionStrategy.UseRangeRule, "*", null)]
    [InlineData("latest", ChannelRevisionSelectionStrategy.UseRangeRule, null, null)]
    public async Task ShouldCreate(string name, ChannelRevisionSelectionStrategy revisionSelectionStrategy, string? rangeRule, Revision? activeRevision)
    {
        var appId = Guid.NewGuid();

        await AddAsync(new App
        {
            Id = appId,
            Name = RandomString(10),
            StorageId = RandomString(10)
        });

        var command = new CreateChannelCommand
        {
            Name = name,
            AppId = appId,
            RevisionSelectionStrategy = revisionSelectionStrategy,
            RangeRule = rangeRule,
            ActiveRevisionId = activeRevision?.Id
        };

        await SendAsync(command);
    }

    [Theory]
    [InlineData("!@#$%^&*(){}[]<>\\|'\";:,./?=+", ChannelRevisionSelectionStrategy.UseRangeRule, "*")]
    [InlineData("latest", ChannelRevisionSelectionStrategy.UseRangeRule, "")]
    public async Task ShouldRaiseValidationException(string name, ChannelRevisionSelectionStrategy revisionSelectionStrategy, string? rangeRule)
    {
        var appId = Guid.NewGuid();

        await AddAsync(new App
        {
            Id = appId,
            Name = RandomString(10),
            StorageId = RandomString(10)
        });

        var command = new CreateChannelCommand
        {
            Name = name,
            AppId = appId,
            RevisionSelectionStrategy = revisionSelectionStrategy,
            RangeRule = rangeRule
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
