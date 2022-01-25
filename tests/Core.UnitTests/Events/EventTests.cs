using System;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using Xunit;

namespace Core.UnitTests.Events;

public class EventTests
{
    [Fact]
    public void AppCreatedEventTest()
    {
        var x = new App();
        Assert.Equal(x, new AppCreatedEvent(x).App);
    }

    [Fact]
    public void AppDeletedEventTest()
    {
        var x = new App();
        Assert.Equal(x, new AppDeletedEvent(x).App);
    }

    [Fact]
    public void ChannelCreatedEventTest()
    {
        var x = new Channel();
        Assert.Equal(x, new ChannelCreatedEvent(x).Channel);
    }

    [Fact]
    public void ChannelDeletedEventTest()
    {
        var x = new Channel();
        Assert.Equal(x, new ChannelDeletedEvent(x).Channel);
    }

    [Fact]
    public void EnvironmentVariableCreatedEventTest()
    {
        var x = new EnvironmentVariable();
        Assert.Equal(x, new EnvironmentVariableCreatedEvent(x).EnvironmentVariable);
    }

    [Fact]
    public void EnvironmentVariableDeletedEventTest()
    {
        var x = new EnvironmentVariable();
        Assert.Equal(x, new EnvironmentVariableDeletedEvent(x).EnvironmentVariable);
    }

    [Fact]
    public void RevisionCreatedEventTest()
    {
        var x = new Revision();
        Assert.Equal(x, new RevisionCreatedEvent(x).Revision);
    }

    [Fact]
    public void RevisionDeletedEventTest()
    {
        var x = new Revision();
        Assert.Equal(x, new RevisionDeletedEvent(x).Revision);
    }

    [Fact]
    public void ActiveRevisionChangedEventTest()
    {
        var oldRevision = new Revision { RevisionNumber = "1.0.0" };
        var newRevision = new Revision { RevisionNumber = "2.0.0" };
        var channel = new Channel();
        channel.ActiveRevisionId = oldRevision.Id;
        channel.ActiveRevisionId = newRevision.Id;
        Assert.Single(channel.DomainEvents);
    }
}
