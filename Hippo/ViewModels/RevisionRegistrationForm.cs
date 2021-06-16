using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels
{
    public class RevisionRegistrationForm: ITraceable
    {
        [Required]
        public Guid? AppId { get; set; }

        public string AppStorageId { get; set; }

        [Required]
        public string RevisionNumber { get; set; }

        public string FormatTrace()
            => $"{nameof(RevisionRegistrationForm)}[appid={AppId}, rev={RevisionNumber}]";
    }
}
