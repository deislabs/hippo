using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels;

public class AppEditForm : ITraceable, IValidatableObject
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Bindle name")]
    public string StorageId { get; set; }

    [Required]
    public string Collaborators { get; set; }

    public string FormatTrace() =>
        $"{nameof(AppEditForm)}[id={Id}, name={Name}, stg={StorageId}]";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Name))
        {
            yield return new ValidationResult(
                    "The application name must not be empty",
                    new[] { nameof(Name) }
                    );
        }

        if (string.IsNullOrEmpty(StorageId))
        {
            yield return new ValidationResult(
                    "The bindle name must not be empty",
                    new[] { nameof(StorageId) }
                    );
        }
    }

    public ICollection<string> ParseCollaborators()
    {
        if (string.IsNullOrWhiteSpace(Collaborators))
        {
            return Array.Empty<string>();
        }

        return Collaborators.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
