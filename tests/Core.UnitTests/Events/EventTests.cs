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
    public void DomainCreatedEventTest()
    {
        var x = new Domain();
        Assert.Equal(x, new DomainCreatedEvent(x).Domain);
    }

    [Fact]
    public void DomainDeletedEventTest()
    {
        var x = new Domain();
        Assert.Equal(x, new DomainDeletedEvent(x).Domain);
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
        var channel = new Channel();
        var oldVersion = "1.0.0";
        var newVersion = "2.0.0";
        channel.ActiveRevision = new Revision { RevisionNumber = oldVersion };
        var @event = new ActiveRevisionChangedEvent(channel, new Revision { RevisionNumber = newVersion });
        Assert.Equal(channel, @event.Channel);
        Assert.Equal(oldVersion, @event.ChangedFrom!.RevisionNumber);
        Assert.Equal(newVersion, @event.ChangedTo.RevisionNumber);
    }
}
