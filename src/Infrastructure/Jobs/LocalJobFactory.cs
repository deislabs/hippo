using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Jobs;

public class LocalJobFactory : IJobFactory
{
    private readonly IConfiguration configuration;

    public LocalJobFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public Job StartNew(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? _domain)
    {
        var job = new LocalJob(configuration, id, bindleId);
        foreach (var e in environmentVariables)
        {
            job.AddEnvironmentVariable(e.Key, e.Value);
        }
        job.Start();
        return job;
    }
}
