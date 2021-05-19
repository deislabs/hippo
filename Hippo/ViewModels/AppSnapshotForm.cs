using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class AppSnapshotForm
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Channel Name")]
        public string ChannelName { get; set; }
    }
}
