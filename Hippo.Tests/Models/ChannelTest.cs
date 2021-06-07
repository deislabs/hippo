using Hippo.Models;
using Xunit;

namespace Hippo.Tests.Models
{
    public class ChannelTest
    {
        [Fact]
        public void RevisionSelectionStrategValuesMatchDatabase()
        {
            // IF YOU GET A FALURE HERE: do not change the test!  Go back and revert
            // the change to the enum value.  The enum values are contractual with the
            // database and MUST NOT CHANGE.
            AssertMatchesDBValue(0, ChannelRevisionSelectionStrategy.UseRangeRule);
            AssertMatchesDBValue(1, ChannelRevisionSelectionStrategy.UseSpecifiedRevision);
        }

        private static void AssertMatchesDBValue(int dbValue, ChannelRevisionSelectionStrategy enumValue)
        {
            // IF YOU GET A FALURE HERE: do not change the test!  Go back and revert
            // the change to the enum value.  The enum values are contractual with the
            // database and MUST NOT CHANGE.
            string mismatchMessage = $"Enum values must match database expectations - {enumValue} should be {dbValue}";
            Assert.True((int)enumValue == dbValue, mismatchMessage);
        }
    }
}
