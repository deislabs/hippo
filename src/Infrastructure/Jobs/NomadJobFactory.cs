using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Jobs;

public class NomadJobFactory : IJobFactory
{
    private readonly IConfiguration configuration;

    public NomadJobFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public Job StartNew(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain)
    {
        var job = new NomadJob(configuration, id, bindleId, domain!);
        foreach (var e in environmentVariables)
        {
            job.AddEnvironmentVariable(e.Key, e.Value);
        }
        job.Start();
        return job;
    }
}
