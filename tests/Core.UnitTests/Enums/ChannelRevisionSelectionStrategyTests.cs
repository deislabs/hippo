using System;
using Hippo.Core.Enums;
using Xunit;

namespace Core.UnitTests.Enums;

public class ChannelRevisionSelectionStrategyTests
{
    /// <summary>
    /// The underlying values here are contractual with the database.
    /// **DO NOT** change the underlying numeric value of any case.
    /// </summary>
    [Fact]
    public void ShouldReturnCorrectRevisionSelectionStrategy()
    {
        Assert.Equal(0, (Convert.ToInt32(ChannelRevisionSelectionStrategy.UseRangeRule)));
        Assert.Equal(1, (Convert.ToInt32(ChannelRevisionSelectionStrategy.UseSpecifiedRevision)));
    }
}
