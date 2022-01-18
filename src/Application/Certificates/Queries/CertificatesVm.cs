namespace Hippo.Application.Certificates.Queries;

public class CertificatesVm
{
    public IList<CertificateDto> Certificates { get; set; } = new List<CertificateDto>();
}
