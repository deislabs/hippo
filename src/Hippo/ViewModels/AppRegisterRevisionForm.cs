using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels;

public class AppRegisterRevisionForm : ITraceable
{
    [Required]
    public Guid Id { get; set; }

    [Display(Name = "Revision to register")]
    public string RevisionNumber { get; set; }

    public string FormatTrace() =>
        $"{nameof(AppRegisterRevisionForm)}[id={Id}]";
}
