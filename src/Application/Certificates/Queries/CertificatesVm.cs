using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Certificates.Queries;

public class CertificatesVm
{
    [Required]
    public IList<CertificateDto> Certificates { get; set; } = new List<CertificateDto>();
}
