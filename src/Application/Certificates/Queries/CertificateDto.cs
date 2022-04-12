using System.ComponentModel.DataAnnotations;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.Certificates.Queries;

public class CertificateDto : IMapFrom<Certificate>
{
    public CertificateDto()
    {
        Channels = new List<ChannelDto>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string PublicKey { get; set; } = "";

    [Required]
    public string PrivateKey { get; set; } = "";

    [Required]
    public IList<ChannelDto> Channels { get; set; }
}
