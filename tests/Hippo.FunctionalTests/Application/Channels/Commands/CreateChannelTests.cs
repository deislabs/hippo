using System;
using System.Threading.Tasks;
using Hippo.Application.Channels.Commands;
using Hippo.Application.Common.Config;
using Hippo.Application.Common.Exceptions;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Microsoft.Extensions.Configuration;
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

     [Fact]
    public async Task ValidateChannelDomainName()
    {
        // regular users cannot create 2 channels per app
        await RunAsAdministratorAsync();

        HippoConfig hippoConfig = new HippoConfig();
        GetConfiguration().GetSection("Hippo").Bind(hippoConfig);

        var app = new App
        {
            Id = Guid.NewGuid(),
            Name = RandomString(10),
            StorageId = RandomString(10)
        };

        await AddAsync(app);

        var command = new CreateChannelCommand
        {
            Name = RandomString(10),
            AppId = app.Id,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
        };

        var channelId = await SendAsync(command);

        var channel = await FindAsync<Channel>(new object[] { channelId });

        Assert.NotNull(channel);

        Assert.Equal($"{app.Name}.{hippoConfig.PlatformDomain}".Replace('_', '-').ToLower(), channel.Domain);

        var command2 = new CreateChannelCommand
        {
            Name = RandomString(10),
            AppId = app.Id,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
        };

        var channelId2 = await SendAsync(command2);

        var channel2 = await FindAsync<Channel>(new object[] { channelId2 });

        Assert.NotNull(channel2);

        Assert.Equal($"{channel2.Name}-{app.Name}.{hippoConfig.PlatformDomain}".Replace('_', '-').ToLower(), channel2.Domain);
    }
}
