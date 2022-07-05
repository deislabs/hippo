using System;
using Hippo.Core.Common;
using Xunit;

namespace Hippo.Core.UnitTests.Common;

public class AuditableEntityTests
{
    private class Auditable : AuditableEntity
    {
    }

    [Fact]
    public void CreatedAndModifiedShouldBeInitialized()
    {
        // TODO: mock out DateTime.UtcNow so we can assert with a little more assurance
        Assert.Equal(DateTime.UtcNow.Year, new Auditable().Created.Year);
        Assert.Equal(DateTime.UtcNow.Month, new Auditable().Created.Month);
        Assert.Equal(DateTime.UtcNow.Day, new Auditable().Created.Day);
        Assert.Equal(DateTime.UtcNow.Hour, new Auditable().Created.Hour);
        Assert.Equal(DateTime.UtcNow.Minute, new Auditable().Created.Minute);

        Assert.Equal(DateTime.UtcNow.Year, new Auditable().LastModified.Year);
        Assert.Equal(DateTime.UtcNow.Month, new Auditable().LastModified.Month);
        Assert.Equal(DateTime.UtcNow.Day, new Auditable().LastModified.Day);
        Assert.Equal(DateTime.UtcNow.Hour, new Auditable().LastModified.Hour);
        Assert.Equal(DateTime.UtcNow.Minute, new Auditable().LastModified.Minute);
    }
}
