using System.ComponentModel.DataAnnotations;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.Certificates.Queries;

public class CertificateRecord : IMapFrom<Certificate>
{
    public CertificateRecord()
    {
        Channels = new List<ChannelItem>();
    }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string PublicKey { get; set; } = "";

    [Required]
    public string PrivateKey { get; set; } = "";

    [Required]
    public IList<ChannelItem> Channels { get; set; }
}
