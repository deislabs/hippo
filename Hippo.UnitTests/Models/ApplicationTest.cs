using System.Collections.Generic;
using System.Linq;
using Hippo.Models;
using Xunit;

namespace Hippo.Tests.Models
{
    public class ApplicationTest
    {
        private static Application TestApplication()
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
            return new Application
            {
                Name = "testapp",
                StorageId = "hippofactory.io/unittestapp",
                Revisions = revisions,
                Channels = new List<Channel>(),
            };
        }

        [Fact]
        public void ReevaluationUpdatesAllChannels()
        {
            var app = TestApplication();
            app.Channels.Add(new Channel { Application = app, RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision, SpecifiedRevision = app.Revisions.ElementAt(2) });
            app.Channels.Add(new Channel { Application = app, RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule, RangeRule = "~1.1" });

            app.ReevaluateActiveRevisions();

            Assert.Equal("0.6.1", app.Channels.ElementAt(0).ActiveRevision.RevisionNumber);
            Assert.Equal("1.1.1", app.Channels.ElementAt(1).ActiveRevision.RevisionNumber);

            app.Revisions.Add(new Revision { RevisionNumber = "1.1.6" });

            app.ReevaluateActiveRevisions();

            Assert.Equal("0.6.1", app.Channels.ElementAt(0).ActiveRevision.RevisionNumber);
            Assert.Equal("1.1.6", app.Channels.ElementAt(1).ActiveRevision.RevisionNumber);
        }
    }
}
