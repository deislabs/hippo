using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Core.Entities;
using Hippo.Core.Enums;

namespace Hippo.Application.Certificates.Queries;

public class CertificateRecord : IMapFrom<Certificate>
{
    public CertificateRecord()
    {
        Channels = new List<ChannelDto>();
    }

    public string? Name { get; set; }

    public string? PublicKey { get; set; }

    public string? PrivateKey { get; set; }

    public IList<ChannelDto> Channels { get; set; }
}
