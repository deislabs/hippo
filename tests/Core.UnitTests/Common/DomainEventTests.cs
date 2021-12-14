using System;
using Hippo.Core.Common;
using Xunit;

namespace Core.UnitTests.Common;

public class DomainEventTests
{
    private class TestEvent : DomainEvent
    {
    }

    [Fact]
    public void CreatedAndModifiedShouldBeInitialized()
    {
        // TODO: mock out DateTime.UtcNow so we can assert with a little more assurance
        Assert.Equal(DateTime.UtcNow.Year, new TestEvent().DateOccurred.Year);
        Assert.Equal(DateTime.UtcNow.Month, new TestEvent().DateOccurred.Month);
        Assert.Equal(DateTime.UtcNow.Day, new TestEvent().DateOccurred.Day);
        Assert.Equal(DateTime.UtcNow.Hour, new TestEvent().DateOccurred.Hour);
        Assert.Equal(DateTime.UtcNow.Minute, new TestEvent().DateOccurred.Minute);
    }
}
