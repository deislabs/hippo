using Hippo.Application.Jobs;

namespace Hippo.Application.Common.Interfaces;

public interface IJobService
{
    public void StartJob(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain);
    public void DeleteJob(string jobName);
    public string[] GetJobLogs(string jobName);
}
