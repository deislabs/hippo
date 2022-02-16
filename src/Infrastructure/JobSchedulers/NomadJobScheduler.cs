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

    public ChannelStatus Start(Channel c)
    {
        var jobName = $"{c.App.Name}-{c.Name}";
        var nomadProgram = NomadBinaryPath();
        var bindleUrl = configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1");
        if (bindleUrl == null)
        {
            throw new ArgumentException("Bindle URL cannot be null.");
        }

        if (c.ActiveRevision == null || string.IsNullOrWhiteSpace(c.ActiveRevision.RevisionNumber))
        {
            logger.LogWarning($"Can't start {c.App.Name}:{c.Name}: no active revision");
            return new ChannelStatus(false, "");
        }

        var bindle = $"{c.App.StorageId}/{c.ActiveRevision.RevisionNumber}";
        var hcl = JobDefinition(c);

        logger.LogTrace($"Starting nomad job {c.App.Name}-{c.Name}");

        var psi = new ProcessStartInfo
        {
            FileName = "nomad",
            Arguments = $"job run -var=\"bindle_id={bindle}\" -var=\"host={c.Domain}\" -var=\"bindle_url={bindleUrl}\" -",
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
                    return new ChannelStatus(false, "");
                }

                process.EnableRaisingEvents = true;
                process.Start();
                process.StandardInput.WriteLine(hcl);
                process.StandardInput.Close();

                Task.WhenAll(ForwardLogs(process.StandardError, $"{c.App.Name}:{c.Name}:nomad:stderr"));

                process.WaitForExit();
                logger.LogTrace($"nomad job {jobName} is ready");
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

        // TODO: fetch job status and the job's listen address
        // for now we'll go ahead and inject dummy data...
        return new ChannelStatus(true, "127.0.0.1");
    }

    public void Stop(Channel c)
    {
        var jobName = $"{c.App.Name}-{c.Name}";
        var psi = new ProcessStartInfo
        {
            FileName = "nomad",
            Arguments = $"job stop {jobName}",
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

                Task.WhenAll(ForwardLogs(process.StandardError, $"{c.App.Name}:{c.Name}:nomad:stderr"));

                process.WaitForExit();
                logger.LogTrace($"nomad job {jobName} has stopped");
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
variable ""bindle_id"" {
  description = ""A bindle id, such as foo/bar/1.2.3""
  type = string
}
variable ""bindle_url"" {
  description = ""The Bindle server URL""
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
      name = """ + name + @"""

      tags = [
        ""traefik.enable=true"",
        ""traefik.http.routers." + name + @".rule=Host(`${var.host}`)"",
      ]
      check {
        name = ""alive""
        type = ""tcp""
        interval = ""10s""
        timeout = ""2s""
      }
    }
    task ""wagi"" {
      driver = ""exec""

      artifact {
        source = ""https://github.com/deislabs/wagi/releases/download/v0.6.2/wagi-v0.6.2-linux-amd64.tar.gz""
        options {
          checksum = ""sha256:232d623e8cd9c5b72e2b76d0668eda0049edbe18f7bb5d6d5f979da2e69d1738""
      }
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
          ""--bindle"", var.bindle_id,
          ""--cache"", ""local/cache.toml"",
          # Use https://github.com/deislabs/wagi-fileserver/pull/10 as a workaround
          # for https://github.com/deislabs/hippo-cli/issues/39
          ""-e"", ""PATH_PREFIX=static/"",
          ""-e"", ""BASE_URL=${var.host}"",
          " + env + @"
        ]
      }

      template {
        data = <<-EOF
        [cache]
        enabled = true
        directory = ""{{ env ""NOMAD_TASK_DIR"" }}""
        # optional
        # see more details at https://docs.wasmtime.dev/cli-cache.html
        cleanup-interval = ""1d""
        files-total-size-soft-limit = ""10Gi""
        EOF

        destination = ""local/cache.toml""
      }
    }
  }
}
";
        return hcl;
    }
}
