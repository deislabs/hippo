using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            // TODO: if we end up with no active revision then we should put the channel into
            // some kind of unhappy status

            return ActiveRevision != previous;
        }

        public ChannelStatus Status()
        {
            if (ActiveRevision == null)
            {
                return ChannelStatus.Unhealthy("No active revision");
            }
            return ChannelStatus.Healthy;
        }

        public ICollection<EnvironmentVariable> GetEnvironmentVariables() =>
            Configuration?.EnvironmentVariables ?? Array.Empty<EnvironmentVariable>();
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

    public enum ChannelHealth
    {
        Healthy,
        Unhealthy,
    }

    public class ChannelStatus
    {
        public ChannelHealth Health { get; }
        public IReadOnlyCollection<string> Messages { get; }

        public static readonly ChannelStatus Healthy = new ChannelStatus(ChannelHealth.Healthy);

        public static ChannelStatus Unhealthy(string message) =>
            new ChannelStatus(ChannelHealth.Unhealthy, new[] { message });

        public ChannelStatus(ChannelHealth health)
            : this(health, Enumerable.Empty<string>()) { }

        public ChannelStatus(ChannelHealth health, IEnumerable<string> messages)
        {
            Health = health;
            Messages = new List<string>(messages).AsReadOnly();
        }
    }
}
