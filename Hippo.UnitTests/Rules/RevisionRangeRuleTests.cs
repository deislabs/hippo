using System.Collections.Generic;
using System.Linq;
using Hippo.Models;
using Hippo.Rules;
using Xunit;

namespace Hippo.Tests.Rules
{
    public class RevisionRangeRuleTests
    {
        // Scenarios:
        // * Plain ol' NPM production releases: ^1.1, ~1.2.3, etc. - uninteresting from a testing
        //   POV because the versioning library does them for us
        // * Most recent prerelease by user: ivan, canary, etc.
        //   - This addresses both the dev inner loop and the CI "trunk tracking" scenario
        // * Most recent prerelease *within a version*. This addresses the scenario where a
        //   developer has hotfix or backport builds against multiple versions e.g. I am
        //   fixing a UI issue in 1.4.1 and a database error in 1.6.0.
        //   - It is reasonable to assume that these channels are likely to be relatively
        //     shortlived, and that the developer will know the *full* version they are
        //     building from. So we need to support 1.4.1-ivan-latest but not e.g.
        //     ^1.4-ivan-latest.
        //   - This *might* fall foul of rebase situations. Alice is working on a hotfix
        //     against 1.4.1. But so is Bob. Bob's hotfix is released as 1.4.2. Now Alice
        //     has to rebase and update her channel definition. BO-RING!
        // * Collaboration on a feature. If Alice and Bob are both working on a spongiforms
        //   feature for 1.4.0, we should be able to set up a channel for the 1.4.0-spongiforms
        //   latest prerelease.  (Alice and Bob may still want individual channels though.)
        //   - This is also relevant for "canary deployment of a feature branch."
        private static readonly List<Revision> _revisions = new string[] {
            "1.1.0",
            "1.1.1",
            "1.1.2",
            "1.1.3",
            "1.1.3-alice-2021.01.01.01.01.01.111",
            "1.1.3-alice-2021.04.04.04.04.04.444",
            "1.1.3-bob-2021.02.02.02.02.02.222",
            "1.1.3-spongiforms-alice-2021.03.03.03.03.03.333",
            "1.1.3-spongiforms-alice-2021.02.02.02.02.02.222",
            "1.1.3-spongiforms-bob-2021.02.02.02.02.02.222",
            "1.1.3-spongiforms-2021.05.05.05.05.05.555",
            "1.1.3-spongiforms-2021.04.04.04.04.04.444",
            "1.1.4-alice-2021.01.02.03.04.05.678",
            "1.2.0",
            "1.2.1",
            "2.0.0",
            "2.1.1",
            "2.1.1-canary-2021.04.04.04.04.04.444",
            "2.1.2-canary-2021.03.03.03.03.03.333",
        }.Select(r => new Revision { RevisionNumber = r }).ToList();

        private static void AssertMatchResult(string expected, RevisionRangeRule rule)
        {
            Assert.Equal(expected, rule.Match(_revisions)?.RevisionNumber);
        }

        [Fact]
        public void NpmPatternsIgnorePrereleases()
        {
            AssertMatchResult("1.2.0", RevisionRangeRule.Parse("1.2.0"));
            AssertMatchResult("1.1.3", RevisionRangeRule.Parse("~1.1"));
            AssertMatchResult("2.1.1", RevisionRangeRule.Parse("~2"));
            AssertMatchResult("1.2.1", RevisionRangeRule.Parse("^1.1"));
        }

        [Fact]
        public void SupportsMostRecentReleaseByUser()
        {
            AssertMatchResult("1.1.3-alice-2021.04.04.04.04.04.444", RevisionRangeRule.Parse("P:*-alice-*"));
            AssertMatchResult("1.1.3-bob-2021.02.02.02.02.02.222", RevisionRangeRule.Parse("P:*-bob-*"));
            AssertMatchResult("2.1.1-canary-2021.04.04.04.04.04.444", RevisionRangeRule.Parse("P:*-canary-*"));
        }

        [Fact]
        public void SupportsMostRecentReleaseByUserWithinAVersion()
        {
            AssertMatchResult("1.1.3-alice-2021.04.04.04.04.04.444", RevisionRangeRule.Parse("P:1.1.3-alice-*"));
            AssertMatchResult("1.1.4-alice-2021.01.02.03.04.05.678", RevisionRangeRule.Parse("P:1.1.4-alice-*"));
        }

        [Fact]
        public void SupportsLongRunningFeatureBranches()
        {
            AssertMatchResult("1.1.3-spongiforms-2021.05.05.05.05.05.555", RevisionRangeRule.Parse("P:*-spongiforms-*"));
        }

        [Fact]
        public void SupportsContributorsInLongRunningFeatureBranches()
        {
            AssertMatchResult("1.1.3-spongiforms-alice-2021.03.03.03.03.03.333", RevisionRangeRule.Parse("P:*-spongiforms-alice-*"));
            AssertMatchResult("1.1.3-spongiforms-bob-2021.02.02.02.02.02.222", RevisionRangeRule.Parse("P:*-spongiforms-bob-*"));
        }
    }
}
