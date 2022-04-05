using System;
using System.Linq;
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

    [Fact]
    public void ActiveRevisionChangedEventTest()
    {
        var oldRevision = new Revision { RevisionNumber = "1.0.0" };
        var newRevision = new Revision { RevisionNumber = "2.0.0" };
        var channel = new Channel();
        channel.ActiveRevisionId = oldRevision.Id;
        channel.ActiveRevisionId = newRevision.Id;
        Assert.Single(channel.DomainEvents);
        Assert.Equal(typeof(ActiveRevisionChangedEvent), channel.DomainEvents.Last().GetType());
        channel.ActiveRevision = oldRevision;
        Assert.Equal(2, channel.DomainEvents.Count);
        Assert.Equal(typeof(ActiveRevisionChangedEvent), channel.DomainEvents.Last().GetType());
    }

    [Fact]
    public void CertificateBindEventTest()
    {
        var oldCertificate = new Certificate { };
        var newCertificate = new Certificate { };
        var channel = new Channel();
        channel.CertificateId = oldCertificate.Id;
        Assert.Single(channel.DomainEvents);
        Assert.Equal(typeof(CertificateBindEvent), channel.DomainEvents.Last().GetType());
        channel.CertificateId = newCertificate.Id;
        Assert.Equal(2, channel.DomainEvents.Count);
        Assert.Equal(typeof(CertificateBindEvent), channel.DomainEvents.Last().GetType());
        channel.Certificate = oldCertificate;
        Assert.Equal(3, channel.DomainEvents.Count);
        Assert.Equal(typeof(CertificateBindEvent), channel.DomainEvents.Last().GetType());
        channel.Certificate = null;
        Assert.Equal(4, channel.DomainEvents.Count);
        Assert.Equal(typeof(CertificateUnbindEvent), channel.DomainEvents.Last().GetType());
    }
}
