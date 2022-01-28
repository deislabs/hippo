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
        Assert.Equal(typeof(ActiveRevisionChangedEvent), channel.DomainEvents.Last().GetType());
        channel.ActiveRevision = oldRevision;
        Assert.Equal(2, channel.DomainEvents.Count);
        Assert.Equal(typeof(ActiveRevisionChangedEvent), channel.DomainEvents.Last().GetType());
    }

    [Fact]
    public void CertificateBindEventTest()
    {
        var oldCertificate = new Certificate {};
        var newCertificate = new Certificate {};
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
