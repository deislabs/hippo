using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Jobs;

public class NomadJobFactory : IJobFactory
{
    private readonly IConfiguration configuration;
    private readonly INomadService _nomadService;

    private List<NomadJob> jobs = new List<NomadJob>();

    public NomadJobFactory(IConfiguration configuration, INomadService nomadService)
    {
        this.configuration = configuration;
        _nomadService = nomadService;
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
            _nomadService.ReloadJob(job);
        }
        else
        {
            job = new NomadJob(configuration, id, bindleId, domain!);
            foreach (var e in environmentVariables)
            {
                job.AddEnvironmentVariable(e.Key, e.Value);
            }
            _nomadService.PostJob(job);
            jobs.Add(job);
        }
        return job;
    }
}
