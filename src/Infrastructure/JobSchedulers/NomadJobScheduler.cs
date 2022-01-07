using System.ComponentModel;
using System.Diagnostics;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hippo.Infrastructure.JobSchedulers;

public class NomadJobScheduler : IJobScheduler
{
    private static readonly IReadOnlyList<string> RUST_TRACE_LEVELS = new List<string> {
        "TRACE", "DEBUG", "INFO", "WARN", "ERROR" // TODO: check
	}.AsReadOnly();

    private readonly ILogger<NomadJobScheduler> logger;

    private readonly IConfiguration configuration;

    public NomadJobScheduler(ILogger<NomadJobScheduler> logger, IConfiguration configuration)
    {
        this.logger = logger;
        this.configuration = configuration;
    }

    public void Start(Channel c)
    {
        var nomadProgram = NomadBinaryPath();
        var bindleUrl = configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1");
        if (bindleUrl == null)
        {
            throw new ArgumentException("Bindle URL cannot be null.");
        }

        if (c.ActiveRevision == null || string.IsNullOrWhiteSpace(c.ActiveRevision.RevisionNumber))
        {
            logger.LogWarning($"Can't start {c.App.Name}:{c.Name}: no active revision");
            return;
        }

        var bindle = $"{c.App.StorageId}/{c.ActiveRevision.RevisionNumber}";
        var hcl = JobDefinition(c);

        logger.LogTrace($"Starting nomad job {c.App.Name}-{c.Name}");

        var psi = new ProcessStartInfo
        {
            FileName = "nomad",
            Arguments = $"job run -var=\"bindle={bindle}\" -var=\"host={c.Domain}\" -var=\"bindle_url={bindleUrl}\" -",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
        };
        try
        {
            using (var process = Process.Start(psi))
            {
                if (process == null)
                {
                    // TODO: probably want to throw an Exception here instead
                    logger.LogError($"Process {psi.FileName} with arguments {psi.Arguments} never started");
                    return;
                }

                process.EnableRaisingEvents = true;
                process.Start();
                process.StandardInput.WriteLine(hcl);
                process.StandardInput.Close();

                Task.WhenAll(ForwardLogs(process.StandardError, $"{c.App.Name}:{c.Name}:nomad:stderr"));

                process.WaitForExit();
                logger.LogTrace($"nomad job {c.App.Name}-{c.Name} is ready");
            }
        }
        catch (Win32Exception e)  // yes, even on Linux
        {
            if (e.Message.Contains("No such file or directory", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("The system cannot find the file specified", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException($"The system cannot find 'nomad'; add 'nomad' to your $PATH or set 'Nomad:BinaryPath' in your appsettings to the correct location.");
            }
            throw;
        }

        throw new NotImplementedException();
    }

    public void Stop(Channel c)
    {
        throw new NotImplementedException();
    }

    private string NomadBinaryPath()
    {
        return configuration.GetValue<string>("Nomad:BinaryPath", (OperatingSystem.IsWindows() ? "nomad.exe" : "nomad"));
    }

    // TODO: deduplicate without incurring a check on every line
    private async Task ForwardLogs(StreamReader source, string streamId)
    {
        string? line;
        while ((line = await source.ReadLineAsync()) != null)
        {
            logger.Log(ConvertLogLevel(line), $"{streamId}: {line}");
        }
    }

    private static LogLevel ConvertLogLevel(string rustTraceLine)
    {
        var rustLevel = ExtractRustTraceLevel(rustTraceLine);
        return rustLevel switch
        {
            "TRACE" => LogLevel.Trace,
            "DEBUG" => LogLevel.Debug,
            "INFO" => LogLevel.Information,
            "WARN" => LogLevel.Warning,
            "ERROR" => LogLevel.Error,
            _ => LogLevel.Warning,
        };
    }

    private static string? ExtractRustTraceLevel(string rustTraceLine) =>
        rustTraceLine.Split(' ').Select(AsRustTraceLevel).FirstOrDefault(s => s != null);

    private static string? AsRustTraceLevel(string fragment) =>
        RUST_TRACE_LEVELS.Where(level => fragment.Contains(level, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

    // ActiveRevision can safely ignore nullability thanks to the null check on line 35    
    private static string JobDefinition(Channel c)
    {
        var name = $"{c.App.Name}-{c.Name}";
        var bindle = $"{c.App.StorageId}/{c.ActiveRevision!.RevisionNumber}";
        var env = String.Join(' ', c.EnvironmentVariables.Select(ev => $"\"--env\", \"{ev.Key}='{ev.Value}'\","));

        var hcl = @"
variable ""bindle"" {
  type = string
}
variable ""bindle_url"" {
  type = string
}
variable ""host"" {
  type = string
}
job """ + name + @""" {
  datacenters = [""dc1""]
  type = ""service""
  group """ + name + @""" {
    count = 1
    network {
      port ""http"" { }
    }
    service {
      port = ""http""
      tags = [
        ""traefik.enable=true"",
        ""traefik.http.routers.http.rule=Host(`${var.host}`)"",
      ]
      check {
        type = ""http""
        path = ""/healthz""
        interval = ""10s""
        timeout = ""2s""
      }
    }
    task ""wagi"" {
      driver = ""raw_exec""
      env {
        RUST_LOG = ""warn,wagi=debug""
        BINDLE_URL = var.bindle_url
        WAGI_LOG_DIR = ""local/log""
      }
      config {
        command = ""wagi""
        args = [
          ""--listen"", ""${NOMAD_IP_http}:${NOMAD_PORT_http}"",
          ""--log-dir"", ""local/log"",
          ""--bindle"", var.bindle,
          " + env + @"
        ]
      }
    }
  }
}
";
        return hcl;
    }
}
