using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Jobs;

public class NomadJobFactory : IJobFactory
{
    private readonly IConfiguration configuration;

    private List<NomadJob> jobs = new List<NomadJob>();

    public NomadJobFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public Job Start(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain)
    {
        var job = jobs.Find(j => j.Id == id);
        if (job is not null)
        {
            job.BindleId = bindleId;
            job.Domain = domain!;
            foreach (var e in environmentVariables)
            {
                job.AddEnvironmentVariable(e.Key, e.Value);
            }
            job.Reload();
        }
        else
        {
            job = new NomadJob(configuration, id, bindleId, domain!);
            foreach (var e in environmentVariables)
            {
                job.AddEnvironmentVariable(e.Key, e.Value);
            }
            job.Start();
            jobs.Add(job);
        }
        return job;
    }
}
