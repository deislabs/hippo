using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Hippo.Rules;

namespace Hippo.Models;

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

    // TODO: doesn't work in memory or with SQLite
    // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint PortID { get; set; }
    public Configuration Configuration { get; set; }

    public ActiveRevisionChange ReevaluateActiveRevision()
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

        if (ActiveRevision?.RevisionNumber == previous?.RevisionNumber)
        {
            return null;
        }

        return new ActiveRevisionChange(previous?.RevisionNumber, ActiveRevision?.RevisionNumber, this);
    }

    public HealthStatus Status()
    {
        if (ActiveRevision == null)
        {
            return HealthStatus.Unhealthy("No active revision");
        }
        return HealthStatus.Healthy;
    }

    // TODO: this will change to the domain when we get the reverse proxy working
    public string ServedOn()
    {
        return (PortID + EphemeralPortRange).ToString(CultureInfo.InvariantCulture) + (Domain == null ? "" : $" or {Domain.Name}");
    }

    public ICollection<EnvironmentVariable> GetEnvironmentVariables() =>
        Configuration?.EnvironmentVariables ?? Array.Empty<EnvironmentVariable>();

    public string ConfigurationSummary()
    {
        var strategy = RevisionSelectionStrategy switch
        {
            ChannelRevisionSelectionStrategy.UseSpecifiedRevision =>
                $"fixed revision {SpecifiedRevision?.RevisionNumber ?? "(none)"}",
            ChannelRevisionSelectionStrategy.UseRangeRule =>
                $"rule {RangeRule}",
            _ => "invalid",
        };
        var domain = Domain?.Name ?? "(none)";

        var sb = new StringBuilder();
        sb.AppendFormat(CultureInfo.InvariantCulture, $"strategy: {strategy}; domain: {domain}");
        return sb.ToString();
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

public record ActiveRevisionChange(
        string ChangedFrom,
        string ChangedTo,
        Channel Channel
        );
