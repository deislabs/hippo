using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hippo.Logging;
using Hippo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hippo.ViewModels
{
    public class AppEditChannelForm : ITraceable
    {
        [Required]
        public Guid ApplicationId { get; set; }
        [Required]
        public Guid ChannelId { get; set; }
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
            $"{GetType().Name}[appid={ApplicationId}, chanid={ChannelId}, rev={SelectedRevisionNumber}]";
    }
}
