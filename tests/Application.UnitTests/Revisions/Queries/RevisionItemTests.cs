using Xunit;
using System.Collections.Generic;
using Hippo.Application.Revisions.Queries;

namespace Hippo.Application.UnitTests.Revisions.Queries;

public class RevisionItemTests
{

    [Fact]
    public void ShouldBeSortedByRevisionNumber()
    {
        var revisions = new List<RevisionItem>()
        {
            new RevisionItem
            {
                RevisionNumber = "1.0.0"
            },
            new RevisionItem
            {
                RevisionNumber = "2.0.0"
            },
            new RevisionItem
            {
                RevisionNumber = "1.0.1"
            }
        };

        revisions.Sort();

        Assert.Equal("1.0.0", revisions[0].RevisionNumber);
        Assert.Equal("1.0.1", revisions[1].RevisionNumber);
        Assert.Equal("2.0.0", revisions[2].RevisionNumber);
    }
}
