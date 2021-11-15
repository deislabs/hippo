using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hippo.Logging;
using Hippo.Models;
using Hippo.OperationData;
using Hippo.Rules;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hippo.ViewModels;

public sealed class AppNewChannelForm : ITraceable, IValidatableObject, ICreateChannelParameters
{
    [Required]
    public Guid Id { get; set; }

    [Display(Name = "Channel name")]
    [Required]
    public string ChannelName { get; set; }

    [Display(Name = "Desired revision selection strategy")]
    public string SelectedRevisionSelectionStrategy { get; set; }
    public IEnumerable<SelectListItem> RevisionSelectionStrategies { get; set; }

    [Display(Name = "Revision to update it to (if UseSpecifiedRevision)")]
    public string SelectedRevisionNumber { get; set; }
    public IEnumerable<SelectListItem> Revisions { get; set; }

    [Display(Name = "Revision rule (if UseRangeRule)")]
    public string SelectedRevisionRule { get; set; }

    [Display(Name = "Environment variables (name=value separated by semicolon or newline")]
    public string EnvironmentVariables { get; set; }

    [Display(Name = "Domain name")]
    public string Domain { get; set; }

    public string FormatTrace() =>
        $"{GetType().Name}[id={Id}, rev={SelectedRevisionNumber}, chan={ChannelName}]";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(SelectedRevisionSelectionStrategy))
        {
            yield return new ValidationResult("Must select a revision strategy", new[] { nameof(SelectedRevisionSelectionStrategy) });
        }

        if (SelectedRevisionSelectionStrategy == Enum.GetName(ChannelRevisionSelectionStrategy.UseSpecifiedRevision))
        {
            if (string.IsNullOrWhiteSpace(SelectedRevisionNumber))
            {
                yield return new ValidationResult(
                        $"Revision number must be specified when fixing a channel to a revision number",
                        new[] { nameof(SelectedRevisionNumber) });
            }

            if (!SemVer.Version.TryParse(SelectedRevisionNumber, out _))
            {
                yield return new ValidationResult(
                        $"Revision number is not in a valid format",
                        new[] { nameof(SelectedRevisionNumber) });
            }

        }

        if (SelectedRevisionSelectionStrategy == Enum.GetName(ChannelRevisionSelectionStrategy.UseRangeRule))
        {
            if (string.IsNullOrWhiteSpace(SelectedRevisionRule))
            {
                yield return new ValidationResult(
                        $"Revision range rule must be specified when not fixing a channel to a revision number",
                        new[] { nameof(SelectedRevisionRule) });
            }


            var ruleError = RevisionRangeRule.Validate(SelectedRevisionRule);
            if (ruleError != null)
            {
                yield return new ValidationResult(
                        $"Revision range rule is not valid rule syntax: {ruleError.Message}",
                        new[] { nameof(SelectedRevisionRule) });
            }

        }

        if (!string.IsNullOrWhiteSpace(EnvironmentVariables))
        {
            var entries = EnvironmentVariables.Split('\n', ';').Select(s => s.Trim());
            var invalidEntries = entries.Where(e => !IsValidEnvVar(e));
            foreach (var invalidEntry in invalidEntries)
            {
                yield return new ValidationResult($"'{invalidEntry} is not in a valid format for an environment variable", new[] { nameof(EnvironmentVariables) });
            }
        }

        // TODO: validate domain
    }

    private static bool IsValidEnvVar(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry))
        {
            return false;
        }
        var bits = entry.Split('=');
        return bits.Length == 2 && !string.IsNullOrWhiteSpace(bits[0]) && !string.IsNullOrWhiteSpace(bits[1]);
    }

    private static Dictionary<string, string> ParseEnvironmentVariables(string text)
    {
        // TODO: assumes validation in web form - should not assume this
        if (string.IsNullOrWhiteSpace(text))
        {
            return new();
        }

        return text.Split('\n', ';')
            .Select(e => e.Trim())
            .Select(e => e.Split('='))
            .ToDictionary(bits => bits[0], bits => bits[1]);
    }

    // Adapters for ICreateChannelParameters
    Guid ICreateChannelParameters.ApplicationId => Id;
    string ICreateChannelParameters.DomainName => Domain;
    Dictionary<string, string> ICreateChannelParameters.EnvironmentVariables =>
        ParseEnvironmentVariables(EnvironmentVariables);
    ChannelRevisionSelectionStrategy ICreateChannelParameters.RevisionSelectionStrategy =>
        Enum.Parse<ChannelRevisionSelectionStrategy>(SelectedRevisionSelectionStrategy);
    string ICreateChannelParameters.RevisionNumber => SelectedRevisionNumber;
    string ICreateChannelParameters.RangeRule => SelectedRevisionRule;
}
