using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hippo.Logging;
using Hippo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hippo.ViewModels
{
    public class AppReleaseForm: ITraceable
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Channel to update")]
        public string SelectedChannelName { get; set; }
        public IEnumerable<SelectListItem> Channels { get; set; }


        [Display(Name = "Revision to update it to")]
        public string SelectedRevisionNumber { get; set; }
        public IEnumerable<SelectListItem> Revisions { get; set; }

        public string FormatTrace() =>
            $"{nameof(AppReleaseForm)}[id={Id}, rev={SelectedRevisionNumber}, chan={SelectedChannelName}]";
    }
}
