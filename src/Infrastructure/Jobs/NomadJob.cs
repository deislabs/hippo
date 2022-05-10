using System.ComponentModel;
using System.Diagnostics;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Jobs;

public class NomadJob : Job
{
    public string BindleId;
    public string Domain;
    public readonly Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
    public readonly string bindleUrl;
    public readonly string nomadBinaryPath;
    public readonly string spinBinaryPath;
    public List<string> datacenters;
    public readonly string driver;
    private Process? process;
    private readonly IConfiguration _configuration;

    public NomadJob(IConfiguration configuration, Guid id, string bindleId, string domain) : base(id)
    {
        _configuration = configuration;
        BindleId = bindleId;
        Domain = domain;
        bindleUrl = configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1");
        nomadBinaryPath = configuration.GetValue<string>("Nomad:BinaryPath", (OperatingSystem.IsWindows() ? "nomad.exe" : "nomad"));
        spinBinaryPath = configuration.GetValue<string>("Spin:BinaryPath", (OperatingSystem.IsWindows() ? "spin.exe" : "spin"));
        datacenters = configuration.GetSection("Nomad:Datacenters").Get<string[]>().ToList();
        driver = configuration.GetValue<string>("Nomad:Driver", (OperatingSystem.IsLinux() ? "exec" : "raw_exec"));
    }

    public void AddEnvironmentVariable(string key, string value)
    {
        environmentVariables[key] = value;
    }
}
