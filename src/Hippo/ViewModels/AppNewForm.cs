using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hippo.OperationData;
using Hippo.Logging;

namespace Hippo.ViewModels;

public sealed class AppNewForm : ITraceable, IValidatableObject, ICreateApplicationParameters
{
    [Required]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Bindle name")]
    public string StorageId { get; set; }

    public string FormatTrace() =>
        $"{nameof(AppNewForm)}[name={Name},stg={StorageId}]";

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

    // Adapter
    string ICreateApplicationParameters.ApplicationName => Name;
}
