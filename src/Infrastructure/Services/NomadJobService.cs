using Fermyon.Nomad.Api;
using Fermyon.Nomad.Client;
using Fermyon.Nomad.Model;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Hippo.Infrastructure.Jobs;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Hippo.Infrastructure.Services;

public class NomadJobService : IJobService
{
    private readonly JobsApi _jobsClient;
    private readonly ClientsApi _clientsClient;
    private readonly IConfiguration _configuration;

    private static string _taskName = "spin";
    private static string _logSource = "stdout";

    public NomadJobService(IConfiguration configuration)
    {
        _configuration = configuration;
        var nomadUrl = configuration.GetValue("Nomad:Url", "http://localhost:4646/v1");
        var nomadSecret = configuration.GetValue("Nomad:Secret", "");

        Configuration config = new Configuration();
        config.BasePath = nomadUrl;
        config.ApiKey.Add("X-Nomad-Token", nomadSecret);

        _jobsClient = new JobsApi(config);
        _clientsClient = new ClientsApi(config);
    }

    public void StartJob(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain)
    {
        var job = new NomadJob(_configuration, id, bindleId, domain!);

        foreach (var e in environmentVariables)
        {
            job.AddEnvironmentVariable(e.Key, e.Value);
        }

        PostJob(job);
    }

    public void DeleteJob(string jobName)
    {
        _jobsClient.DeleteJob(jobName);
    }

    public string[] GetJobLogs(string jobName)
    {
        var allocationId = _jobsClient.GetJobAllocations(jobName: jobName)
            .OrderByDescending(a => a.CreateTime)
            .Select(a => a.ID)
            .FirstOrDefault();

        if (allocationId is null)
        {
            return new string[] { };
        }

        var allocationData = _clientsClient.GetAllocationLogs(_taskName, _logSource, allocationId).Data.ToString();
        byte[] data = Convert.FromBase64String(allocationData);
        return Encoding.UTF8.GetString(data).Split("\n");
    }

    private void PostJob(Application.Jobs.Job job)
    {
        var nomadJob = job as NomadJob;

        if (nomadJob is null)
        {
            throw new ArgumentException("Job must be of type NomadJob.");
        }

        var jobRegisterRequest = GenerateJobRegisterRequest(nomadJob);
        _jobsClient.PostJob(nomadJob.Id.ToString(), jobRegisterRequest);
    }

    private JobRegisterRequest GenerateJobRegisterRequest(NomadJob nomadJob)
    {
        return new JobRegisterRequest(job: new Fermyon.Nomad.Model.Job
        {
            Name = nomadJob.Id.ToString(),
            ID = nomadJob.Id.ToString(),
            Datacenters = nomadJob.datacenters,
            Type = "service",
            TaskGroups = new List<TaskGroup>
            {
                new TaskGroup
                {
                    Networks = new List<NetworkResource>
                    {
                        GenerateJobNetworkResources()
                    },
                    Name = nomadJob.Id.ToString(),
                    Services = new List<Service>
                    {
                        GenerateJobService(nomadJob)
                    },
                    Tasks = new List<Fermyon.Nomad.Model.Task>
                    {
                        GenerateJobTask(nomadJob)
                    },
                }
            }

        });
    }

    private NetworkResource GenerateJobNetworkResources()
    {
        return new NetworkResource
        {
            DynamicPorts = new List<Port>
            {
                new Port
                {
                    Label = "http",
                }
            }
        };
    }

    private Service GenerateJobService(NomadJob nomadJob)
    {
        var entrypoint = _configuration.GetValue<string>("Nomad:Traefik:Entrypoint");

        var certresolver = _configuration.GetValue<string>("Nomad:Traefik:CertResolver");

        var tags = new List<string>
        {
            "traefik.enable=true",
            "traefik.http.routers." + nomadJob.Id + @".rule=Host(`" + nomadJob.Domain + "`)",
        };

        if (!string.IsNullOrEmpty(entrypoint))
        {
            tags.Add("traefik.http.routers." + nomadJob.Id + @".entryPoints=" + entrypoint);
        }

        if (!string.IsNullOrEmpty(certresolver))
        {
            tags.Add("traefik.http.routers." + nomadJob.Id + @".tls=true");
            tags.Add("traefik.http.routers." + nomadJob.Id + @".tls.certresolver=" + certresolver);
            tags.Add("traefik.http.routers." + nomadJob.Id + @".tls.domains[0].main=" + nomadJob.Domain);
        }

        var serviceProvider = _configuration.GetValue("Nomad:ServiceProvider", "consul");

        var service = new Service
        {
            PortLabel = "http",
            Name = nomadJob.Id.ToString(),
            Tags = tags,
            Provider = serviceProvider,
        };

        if (serviceProvider == "consul")
        {
            service.Checks = new List<ServiceCheck>
            {
                new ServiceCheck
                {
                    Name = "alive",
                    Type = "tcp",
                    Interval = 10000000000,
                    Timeout = 2000000000
                }
            };

        }
        return service;
    }

    private Fermyon.Nomad.Model.Task GenerateJobTask(NomadJob nomadJob)
    {
        return new Fermyon.Nomad.Model.Task
        {
            Name = _taskName,
            Driver = nomadJob.driver,
            Env = new Dictionary<string, string>
            {
                { "RUST_LOG", "warn,spin=debug" },
                { "BINDLE_URL", nomadJob.bindleUrl },
                { "SPIN_LOG_DIR", "local/log" }
            },
            Config = new Dictionary<string, object>
            {
                { "command", nomadJob.spinBinaryPath },
                { "args", new List<string> { "up", "--listen", "${NOMAD_ADDR_http}", "--follow-all", "--bindle", nomadJob.BindleId } }
            }
        };
    }

    public IEnumerable<Application.Jobs.Job>? GetJobs()
    {
        try
        {
            var jobs = _jobsClient.GetJobs()
                .Where(job => Guid.TryParse(job.ID, out _))
                .Select(job => new NomadJob(_configuration, Guid.Parse(job.ID),
                    string.Empty,
                    string.Empty,
                    Enum.Parse<JobStatus>(FormatNomadJobStatus(job.Status))))
                .ToList();

            return jobs;
        }
        catch
        {
            return null;
        }
    }

    public Application.Jobs.Job? GetJob(string jobName)
    {
        try
        {
            var job = _jobsClient.GetJob(jobName);

            return new NomadJob(_configuration,
                Guid.Parse(job.ID),
                string.Empty,
                string.Empty,
                Enum.Parse<JobStatus>(FormatNomadJobStatus(job.Status)));
        }
        catch
        {
            return null;
        }
    }

    private string FormatNomadJobStatus(string status)
    {
        return char.ToUpper(status[0]) + status[1..];
    }
}
