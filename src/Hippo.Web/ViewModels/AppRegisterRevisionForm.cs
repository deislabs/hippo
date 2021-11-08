using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hippo.Core.Interfaces;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hippo.Web.ViewModels
{
    public class AppRegisterRevisionForm : ITraceable
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Revision to register")]
        public string RevisionNumber { get; set; }

        public string FormatTrace() =>
            $"{nameof(AppRegisterRevisionForm)}[id={Id}]";
    }
}
