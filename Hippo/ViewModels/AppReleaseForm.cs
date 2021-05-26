using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels
{
    public class AppReleaseForm: ITraceable
    {
        [Required]
        public Guid Id { get; set; }

        public string Revision { get; set; }

        [Display(Name = "Channel Name")]
        public string ChannelName { get; set; }

        public string FormatTrace() =>
            $"{nameof(AppReleaseForm)}[id={Id}, rev={Revision}, chan={ChannelName}]";
    }
}
