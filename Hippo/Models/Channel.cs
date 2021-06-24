using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Hippo.Rules;
using Nett;

namespace Hippo.Models
{
    public class Channel : BaseEntity
    {
        public const int EphemeralPortRange = 32768;

        public string Name { get; set; }

        public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

        public Revision SpecifiedRevision { get; set; }
        public string RangeRule { get; set; }

        public Revision ActiveRevision { get; set; }

        public Application Application { get; set; }
        public Domain Domain { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint PortID { get; set; }
        public Configuration Configuration { get; set; }

        public bool ReevaluateActiveRevision()
        {
            var previous = ActiveRevision;

            switch (RevisionSelectionStrategy)
            {
                case ChannelRevisionSelectionStrategy.UseSpecifiedRevision:
                    ActiveRevision = SpecifiedRevision;
                    break;
                case ChannelRevisionSelectionStrategy.UseRangeRule:
                    ActiveRevision = RevisionRangeRule.Parse(RangeRule).Match(Application.Revisions);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown revision strategy {RevisionSelectionStrategy}");

            }
            // TODO: should this trigger a redeploy?

            return ActiveRevision != previous;
        }

        // TODO: bikeshed about this function name with Ivan
        public string UniqueName()
        {
            return $"{Application.Name}-{Name}";
        }
    }

    /// <summary>
    /// The strategy to use to select a revision for the Channel
    /// </summary>
    public enum ChannelRevisionSelectionStrategy
    {
        // IMPORTANT: The underlying values here are contractual with the database.
        // **DO NOT** change the underlying numeric value of any case.
        /// <summary>
        /// Use a range rule to select the most appropriate revision for the channel
        /// </summary>
        UseRangeRule = 0,
        /// <summary>
        /// Use a specific revision version for the channel
        /// </summary>
        UseSpecifiedRevision = 1,
    }
}
