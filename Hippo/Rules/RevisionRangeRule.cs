using System.Collections.Generic;
using System.Linq;
using Hippo.Models;
using SemVer;

namespace Hippo.Rules
{
    public class RevisionRangeRule
    {
        private Range _range;

        public static RevisionRangeRule Parse(string rule)
        {
            return new RevisionRangeRule(new Range(rule));
        }

        public RevisionRangeRule(Range range)
        {
            _range = range;
        }

        public Revision Match(ICollection<Revision> candidates)
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
}