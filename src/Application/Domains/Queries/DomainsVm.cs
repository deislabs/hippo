namespace Hippo.Application.Domains.Queries;

public class DomainsVm
{
    public IList<DomainDto> Domains { get; set; } = new List<DomainDto>();
}
