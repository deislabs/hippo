using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Jobs;

public class LocalJobFactory : IJobFactory
{
    private readonly IConfiguration configuration;

    private List<LocalJob> jobs = new List<LocalJob>();

    public LocalJobFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public Job Start(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? _domain)
    {
        var job = jobs.Find(j => j.Id == id);
        if (job is not null)
        {
            job.BindleId = bindleId;
            foreach (var e in environmentVariables)
            {
                job.AddEnvironmentVariable(e.Key, e.Value);
            }
            job.Reload();
        }
        else
        {
            job = new LocalJob(configuration, id, bindleId);
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
