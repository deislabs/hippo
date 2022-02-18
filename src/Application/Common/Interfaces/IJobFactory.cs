using Hippo.Application.Jobs;

namespace Hippo.Application.Common.Interfaces;

public interface IJobFactory
{
    Job StartNew(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain);
}
