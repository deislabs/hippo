using System;
using Hippo.Core.Exceptions;
using Xunit;

namespace Core.UnitTests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void InvalidRevisionRangeRuleExceptionTest()
    {
        Assert.Equal("Range rule \"foo\" is invalid.", new InvalidRevisionRangeRuleException("foo").Message);
    }
}
