using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Core.Interfaces;


namespace Hippo.Web.ViewModels
{
    public class RevisionRegistrationForm : ITraceable
    {
        [Required]
        public Guid AppId { get; set; }

        [Required]
        public string RevisionNumber { get; set; }

        public string FormatTrace()
            => $"{nameof(RevisionRegistrationForm)}[appid={AppId}, rev={RevisionNumber}]";
    }
}
