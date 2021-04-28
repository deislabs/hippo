using System;
using System.ComponentModel.DataAnnotations;

namespace Hippo.ViewModels
{
    public class AppReleaseForm
    {
        [Required]
        public Guid Id { get; set; }

        public string Revision { get; set; }

        public string ChannelName { get; set; }
    }
}
