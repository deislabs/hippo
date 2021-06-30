using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hippo.Logging;
using Hippo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hippo.ViewModels
{
    public class AppNewChannelForm : ITraceable, IValidatableObject
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

            // TODO: validate that we have a revision or rule according to chosen strategy

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
    }
}
