using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class AppReleaseForm
    {
        [Required]
        public Guid Id { get; set; }

        public string Revision { get; set; }

        [Display(Name = "Channel Name")]
        public string ChannelName { get; set; }
    }
}
