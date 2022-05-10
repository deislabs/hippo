using Hippo.Application.Jobs;

namespace Hippo.Application.Common.Interfaces;

public interface INomadService
{
    public void StartJob(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain);
    public void DeleteJob(string jobId);
}
