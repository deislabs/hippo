using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Hippo.Models
{
    public class Revision : BaseEntity
    {
        public virtual Application Application { get; set; }

        // This is the revision number that gets composed with the Application.StorageId
        // to get the bindle ID.  E.g. this might be "1.4.0" or "1.1.5-prerelease2".
        [Required]
        public string RevisionNumber { get; set; }

        public IEnumerable<Channel> ActiveOn() =>
            Application.Channels.Where(c => c.ActiveRevision?.RevisionNumber == RevisionNumber);
    }
}
