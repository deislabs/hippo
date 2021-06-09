using System.Collections.Generic;
using System.Linq;
using Hippo.Models;
using Xunit;

namespace Hippo.Tests.Models
{
    public class ChannelTest
    {
        private static Channel TestChannel()
        {
            var revisions = new List<Revision>
            {
                new Revision { RevisionNumber = "0.5.0"},
                new Revision { RevisionNumber = "0.6.0"},
                new Revision { RevisionNumber = "0.6.1"},
                new Revision { RevisionNumber = "1.0.0"},
                new Revision { RevisionNumber = "1.0.1"},
                new Revision { RevisionNumber = "1.0.2"},
                new Revision { RevisionNumber = "1.1.0"},
                new Revision { RevisionNumber = "1.1.1"},
                new Revision { RevisionNumber = "1.2.0"},
                new Revision { RevisionNumber = "2.0.0-alice-2021-01.01.01.23.45.678"},
                new Revision { RevisionNumber = "2.0.0-betty-2021.01.01.01.23.45.678"},
                new Revision { RevisionNumber = "2.0.0-canary-2021.01.23.01.23.45.678"},
                new Revision { RevisionNumber = "2.0.0-canary-2021.01.24.01.12.45.678"},
                new Revision { RevisionNumber = "2.0.0-beta1"},
                new Revision { RevisionNumber = "2.0.0-beta2"},
                new Revision { RevisionNumber = "2.0.0-rc1"},
                new Revision { RevisionNumber = "2.0.0-rc3"},
                new Revision { RevisionNumber = "2.0.0"},
                new Revision { RevisionNumber = "2.0.1"},
                new Revision { RevisionNumber = "2.1.0"},
                new Revision { RevisionNumber = "2.2.0"},
            };
            return new Channel
            {
                Name = "test",
                Application = new Application
                {
                    Name = "testapp",
                    StorageId = "hippos.rocks/unittestapp",
                    Revisions = revisions,
                },
            };
        }

        [Fact]
        public void EvaluatingSpecifiedRevisionActivatesThatRevision()
        {
            var c = TestChannel();
            c.RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision;
            c.SpecifiedRevision = c.Application.Revisions.ElementAt(1);

            c.ReevaluateActiveRevision();

            Assert.Equal("0.6.0", c.ActiveRevision.RevisionNumber);
        }

        [Fact]
        public void EvaluatingPatchVersionRuleActivatesHighestMatchingRevision()
        {
            var c = TestChannel();
            c.RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule;
            c.RangeRule = "1.0.*";

            c.ReevaluateActiveRevision();
            Assert.Equal("1.0.2", c.ActiveRevision.RevisionNumber);

            c.Application.Revisions.Add(new Revision { RevisionNumber = "1.0.4" });
            c.ReevaluateActiveRevision();
            Assert.Equal("1.0.4", c.ActiveRevision.RevisionNumber);
        }

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
