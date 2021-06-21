#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Hippo.Models;
using SemVer;
using Range = SemVer.Range;

namespace Hippo.Rules
{
    public abstract class RevisionRangeRule
    {
        public static RevisionRangeRule Parse(string? rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (rule.StartsWith("P:", StringComparison.InvariantCultureIgnoreCase))
            {
                return new PrereleaseRevisionRangeRule(rule);
            }
            return new NpmRevisionRangeRule(rule);
        }

        public abstract Revision? Match(ICollection<Revision> candidates);

        private class NpmRevisionRangeRule : RevisionRangeRule
        {
            private readonly Range _range;

            public NpmRevisionRangeRule(string rule)
            {
                _range = new Range(rule);
            }

            public override Revision? Match(ICollection<Revision> candidates)
            {
                var candidatesByVersion = candidates.ToDictionary(c => c.RevisionNumber, c => c);
                var maxSatisfying = _range.MaxSatisfying(candidatesByVersion.Keys);

                if (maxSatisfying == null)
                {
                    return null;
                }
                return candidatesByVersion[maxSatisfying];
            }
        }

        private class PrereleaseRevisionRangeRule : RevisionRangeRule
        {
            private readonly Range _versionRange;
            private readonly string _prereleasePrefix;

            public PrereleaseRevisionRangeRule(string rule)
            {
                // We expect this to be of the form P:(*|npmpat)-<name>-*

                // TODO: this is a bit horrid - drop this bit or implement globbing
                if (!rule.EndsWith("-*", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException("patterns after name not supported", nameof(rule));
                }

                var body = rule.Substring(2, rule.Length - 4);

                var splitAt = body.IndexOf('-');
                var versionPattern = body.Substring(0, splitAt);
                var prereleasePattern = body.Substring(splitAt + 1);

                if (versionPattern.Length == 0 || prereleasePattern.Length == 0)
                {
                    throw new ArgumentException("rule must have something before and after the hyphen", nameof(rule));
                }

                _versionRange = new Range(versionPattern);
                _prereleasePrefix = prereleasePattern;
            }

            public override Revision? Match(ICollection<Revision> candidates)
            {
                var candidatesByVersion =
                    from c in candidates
                    from pr in ParsePrerelease(c.RevisionNumber)
                    where pr.Prerelease.StartsWith(_prereleasePrefix, StringComparison.InvariantCultureIgnoreCase)
                    where _versionRange.IsSatisfied(pr.Version)
                    orderby pr.Prerelease descending
                    select new { Revision = c, Version = pr.Version, Prerelease = pr.Prerelease };

                return candidatesByVersion.FirstOrDefault()?.Revision;
            }

            private IEnumerable<(string Version, string Prerelease)> ParsePrerelease(string revisionNumber)
            {
                var prereleaseParse = revisionNumber.IndexOf('-');
                if (prereleaseParse <= 0)
                {
                    yield break;
                }
                yield return (revisionNumber.Substring(0, prereleaseParse), revisionNumber.Substring(prereleaseParse + 1));
            }
        }
    }
}
