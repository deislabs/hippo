using Hippo.Application.Jobs;

namespace Hippo.Application.Common.Interfaces;

public interface INomadService
{
    public Guid PostJob(Job nomadJob);
    public string GetJobStatus(string jobId);
    public bool DoesJobExist(string jobId);
    public void DeleteJob(string jobId);
    public void ReloadJob(Job job);
}
