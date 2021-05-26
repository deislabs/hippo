using System;
using System.ComponentModel.DataAnnotations;
using Hippo.Logging;

namespace Hippo.ViewModels
{
    public class ReleaseUploadForm: ITraceable
    {
        [Required]
        public Guid AppId { get; set; }

        [Required]
        public string Revision { get; set; }

        [Required]
        [Display(Name = "Upload URL")]
        public string UploadUrl { get; set; }

        public string FormatTrace()
            => $"{nameof(ReleaseUploadForm)}[appid={AppId}, rev={Revision}, url={UploadUrl}]";
    }
}
