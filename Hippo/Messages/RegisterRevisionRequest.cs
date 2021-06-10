using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.Messages
{
    public class RegisterRevisionRequest: ITraceable
    {
        public Guid? AppId { get; set; }

        public string AppStorageId { get; set; }

        [Required]
        public string RevisionNumber { get; set; }

        public string FormatTrace()
            => $"{GetType().Name}[appid={AppId}, stgid={AppStorageId}, rev={RevisionNumber}]";
    }
}
