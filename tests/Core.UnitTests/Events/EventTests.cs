using System;
using System.Linq;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using Xunit;

namespace Hippo.Core.UnitTests.Events;

public class EventTests
{
    [Fact]
    public void AppCreatedEventTest()
    {
        var x = new App();
        Assert.Equal(x, new CreatedEvent<App>(x).Entity);
    }

    [Fact]
    public void AppDeletedEventTest()
    {
        var x = new App();
        Assert.Equal(x, new DeletedEvent<App>(x).Entity);
    }

    [Fact]
    public void ChannelCreatedEventTest()
    {
        var x = new Channel();
        Assert.Equal(x, new CreatedEvent<Channel>(x).Entity);
    }

    [Fact]
    public void ChannelDeletedEventTest()
    {
        var x = new Channel();
        Assert.Equal(x, new DeletedEvent<Channel>(x).Entity);
    }

    [Fact]
    public void EnvironmentVariableCreatedEventTest()
    {
        var x = new EnvironmentVariable();
        Assert.Equal(x, new CreatedEvent<EnvironmentVariable>(x).Entity);
    }

    [Fact]
    public void EnvironmentVariableDeletedEventTest()
    {
        var x = new EnvironmentVariable();
        Assert.Equal(x, new DeletedEvent<EnvironmentVariable>(x).Entity);
    }

    [Fact]
    public void RevisionCreatedEventTest()
    {
        var x = new Revision();
        Assert.Equal(x, new CreatedEvent<Revision>(x).Entity);
    }

    [Fact]
    public void RevisionDeletedEventTest()
    {
        var x = new Revision();
        Assert.Equal(x, new DeletedEvent<Revision>(x).Entity);
    }
}
