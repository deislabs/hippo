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
    private readonly Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
    private readonly string bindleUrl;
    private readonly string nomadBinaryPath;
    private readonly string datacenters;
    private Process? process;

    public NomadJob(IConfiguration configuration, Guid id, string bindleId, string domain) : base(id)
    {
        BindleId = bindleId;
        Domain = domain;
        bindleUrl = configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1");
        nomadBinaryPath = configuration.GetValue<string>("Nomad:BinaryPath", (OperatingSystem.IsWindows() ? "nomad.exe" : "nomad"));
        datacenters = string.Join(", ", configuration.GetSection("Nomad:Datacenters").Get<string[]>().Select(item => "\"" + item + "\""));
    }

    public void AddEnvironmentVariable(string key, string value)
    {
        environmentVariables[key] = value;
    }

    public override void Run()
    {
        var psi = new ProcessStartInfo
        {
            FileName = nomadBinaryPath,
            Arguments = $"job run -var=\"bindle_id={BindleId}\" -var=\"host={Domain}\" -var=\"bindle_url={bindleUrl}\" -",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        try
        {
            process = Process.Start(psi);
            if (process == null)
            {
                throw new JobFailedException("Job never started");
            }

            process.Start();
            process.StandardInput.WriteLine(HclJobDefinition());
            process.StandardInput.Close();
            _status = JobStatus.Running;

            // kill the process if the calling thread cancels the Job.
            cancellationToken.Register(() =>
            {
                process.Kill();
                try
                {
                    Stop();
                }
                catch (Exception) { }
                _status = JobStatus.Canceled;
            });
        }
        catch (Win32Exception e)  // yes, even on Linux
        {
            if (e.Message.Contains("No such file or directory", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("The system cannot find the file specified", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException($"The system cannot find '{nomadBinaryPath}'; add '{nomadBinaryPath}' to your $PATH or set 'Nomad:BinaryPath' in your appsettings to the correct location.");
            }
            throw;
        }
    }

    public override void Reload()
    {
        if (IsRunning)
        {
            process?.Kill();
            Run();
        }
    }

    public override void Stop()
    {
        // no need to tell nomad to stop if the job hasn't started
        if (IsWaitingToRun || IsCompleted)
        {
            return;
        }

        var psi = new ProcessStartInfo
        {
            FileName = "nomad",
            Arguments = $"job stop {Id}",
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
                    throw new JobFailedException("Job failed to stop");
                }

                process.EnableRaisingEvents = true;
                process.Start();
                process.WaitForExitAsync(cancellationToken).Wait();
                _status = JobStatus.Completed;
            }
        }
        catch (Win32Exception e)  // yes, even on Linux
        {
            if (e.Message.Contains("No such file or directory", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("The system cannot find the file specified", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException($"The system cannot find '{nomadBinaryPath}'; add '{nomadBinaryPath}' to your $PATH or set 'Nomad:BinaryPath' in your appsettings to the correct location.");
            }
            throw;
        }
    }

    private string HclJobDefinition()
    {
        var env = String.Join(' ', environmentVariables.Select(ev => $"\"--env\", \"{ev.Key}='{ev.Value}'\","));

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
job """ + Id + @""" {
  datacenters = [" + datacenters + @"]
  type = ""service""
  group """ + Id + @""" {
    count = 1
    network {
      port ""http"" { }
    }
    service {
      port = ""http""
      name = """ + Id + @"""

      tags = [
        ""traefik.enable=true"",
        ""traefik.http.routers." + Id + @".rule=Host(`${var.host}`)"",
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
