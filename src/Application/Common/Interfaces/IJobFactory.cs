using Hippo.Application.Jobs;

namespace Hippo.Application.Common.Interfaces;

public interface IJobFactory
{
    Job Start(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain);
}
