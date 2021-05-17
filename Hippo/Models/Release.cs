using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hippo.Models
{
    public class Release: BaseEntity
    {
        public virtual Application Application { get; set; }

        // TODO: is this something we can infer from the UploadUrl?
        // e.g. bindle:hippos.rocks/myapp/1.0.0
        //
        // If so we should convert this to a function and parse the UploadUrl to fetch this information.
        [Required]
        public string Revision { get; set; }

        [Required]
        public string UploadUrl { get; set; }
    }
}
