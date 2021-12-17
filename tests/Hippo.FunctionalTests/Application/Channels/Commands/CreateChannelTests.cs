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
    public async Task ShouldRequireUniqueDomain()
    {
        await SendAsync(new CreateChannelCommand
        {
            Name = "development",
            Domain = "myapp.example.com",
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            ActiveRevision = null
        });

        var command = new CreateChannelCommand
        {
            Name = "production",
            Domain = "myapp.example.com",
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            ActiveRevision = null
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Theory]
    [InlineData("development", "example.com", ChannelRevisionSelectionStrategy.UseRangeRule, "*", null)]
    [InlineData("development", "myapp.example.com", ChannelRevisionSelectionStrategy.UseRangeRule, "*", null)]
    public async Task ShouldCreate(string name, string domain, ChannelRevisionSelectionStrategy revisionSelectionStrategy, string rangeRule, Revision? activeRevision)
    {
        var command = new CreateChannelCommand
        {
            Name = name,
            Domain = domain,
            RevisionSelectionStrategy = revisionSelectionStrategy,
            RangeRule = rangeRule,
            ActiveRevision = activeRevision
        };

        await SendAsync(command);
    }

    [Theory]
    [InlineData("!@#$%^&*(){}[]<>\\|'\";:,./?=+")]
    public async Task ShouldValidateName(string name)
    {
        var command = new CreateChannelCommand
        {
            Name = name,
            Domain = "myapp.example.com",
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            ActiveRevision = null
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }

    [Theory]
    [InlineData("myapp!example!com")]
    public async Task ShouldValidateDomain(string domain)
    {
        var command = new CreateChannelCommand
        {
            Name = "development",
            Domain = domain,
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            ActiveRevision = null
        };

        await Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(command));
    }
}
