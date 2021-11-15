using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Hippo.Models;

public class Revision : BaseEntity
{
    public virtual Application Application { get; set; }

    // This is the revision number that gets composed with the Application.StorageId
    // to get the bindle ID.  E.g. this might be "1.4.0" or "1.1.5-prerelease2".
    [Required]
    public string RevisionNumber { get; set; }

    public IEnumerable<Channel> ActiveOn() =>
        Application.Channels.Where(c => c.ActiveRevision?.RevisionNumber == RevisionNumber);

    public string OrderKey()
    {
        if (SemVer.Version.TryParse(RevisionNumber, out var version))
        {
            return $"{version.Major:D9}{version.Minor:D9}{version.Patch:D9}{RevisionNumber}";
        }
        return RevisionNumber;
    }
}
