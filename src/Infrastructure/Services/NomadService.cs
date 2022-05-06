using Deislabs.Bindle;
using Fermyon.Nomad.Api;
using Fermyon.Nomad.Client;
using Fermyon.Nomad.Model;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Hippo.Infrastructure.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Services;

public class NomadService : INomadService
{
    private readonly JobsApi _client;
    private readonly IConfiguration _configuration;

    public NomadService(IConfiguration configuration)
    {
        _configuration = configuration;
        var nomadUrl = configuration.GetValue("Nomad:Url", "http://localhost:4646/v1");
        var nomadSecret = configuration.GetValue("Nomad:Secret", "bd8c4f74-ad8c-364d-e5d3-e130bf1ea865");

        Configuration config = new Configuration();
        config.BasePath = nomadUrl;
        config.ApiKey.Add("X-Nomad-Token", nomadSecret);

        _client = new JobsApi(config);
    }

    public Guid PostJob(Application.Jobs.Job job)
    {
        var entrypoint = _configuration.GetValue<string>("Nomad:Traefik:Entrypoint");
        var certresolver = _configuration.GetValue<string>("Nomad:Traefik:CertResolver");

        var nomadJob = job as NomadJob;
        var jobRegisterRequest = new JobRegisterRequest(job: new Fermyon.Nomad.Model.Job
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
                        new NetworkResource
                        {
                            DynamicPorts = new List<Port>
                            {
                                new Port
                                {
                                    Label = "http",
                                }
                            }
                        }
                    },
                    Name = nomadJob.Id.ToString(),
                    Services = new List<Service>
                    {
                        new Service
                        {
                            PortLabel = "http",
                            Name = nomadJob.Id.ToString(),
                            Tags = new List<string>
                            {
                                "traefik.enable=true",
                                "traefik.http.routers." + nomadJob.Id + @".rule=Host(`" + nomadJob.Domain + "`)",
                                "traefik.http.routers." + nomadJob.Id + @".tls.entryPoints=" + entrypoint,
                                "traefik.http.routers." + nomadJob.Id + @".tls=false",
                                "traefik.http.routers." + nomadJob.Id + @".tls.certresolver=" + certresolver,
                                "traefik.http.routers." + nomadJob.Id + @".tls.domains[0].main=" + nomadJob.Domain + ""
                            },
                            Checks = new List<ServiceCheck>
                            {
                                new ServiceCheck
                                {
                                    Name = "alive",
                                    Type = "tcp",
                                    Interval = 1000000000, // is this in mseconds ?? 
                                    Timeout = 2000000000
                                }
                            }
                        }
                    },
                    Tasks = new List<Fermyon.Nomad.Model.Task>
                    {
                        new Fermyon.Nomad.Model.Task
                        {
                            Name = "spin",
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
                                { "args", new List<string> { "up", "--listen", "[${NOMAD_IP_http}]:${NOMAD_PORT_http}", "--log-dir", "local/log", "--bindle", nomadJob.BindleId } }
                            }
                        }
                    }
                }
            }

        });
        var result = _client.PostJob(nomadJob.Id.ToString(), jobRegisterRequest);
        return new Guid();
    }

    public void DeleteJob(string jobName)
    {
        _client.DeleteJob(jobName);
    }

    public void ReloadJob(Application.Jobs.Job job)
    {
        _client.DeleteJob(job.Id.ToString());
        PostJob(job);
    }

    public string GetJobStatus(string jobName)
    {
        var job = _client.GetJob(jobName);

        return job?.Status;
    }
}
