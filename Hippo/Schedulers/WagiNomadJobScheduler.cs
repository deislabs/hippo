using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.Proxies;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public class WagiNomadJobScheduler : InternalScheduler
    {
        public WagiNomadJobScheduler(ILogger<WagiNomadJobScheduler> logger, IReverseProxy reverseProxy, IHostEnvironment env)
            : base(logger, reverseProxy, env)
        {
        }

        public override void Start(Channel c)
        {
            if (c.ActiveRevision == null || string.IsNullOrWhiteSpace(c.ActiveRevision.RevisionNumber))
            {
                _logger.LogWarning($"Can't start {c.Application.Name}:{c.Name}: no active revision");
                return;
            }

            var bindle = $"{c.Application.StorageId}/{c.ActiveRevision.RevisionNumber}";
            var hcl = JobDefinition(c);

            // Console.WriteLine(hcl);

            _logger.LogTrace($"Starting nomad job {c.Application.Name}-{c.Name}");

            var psi = new ProcessStartInfo
            {
                FileName = "nomad",
                Arguments = $"job run -var=\"bindle={bindle}\" -var=\"host={c.Domain.Name}\" -var=\"bindle_url={_bindleUrl}\" -",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
            try
            {
                using (var process = Process.Start(psi))
                {
                    process.EnableRaisingEvents = true;
                    process.Start();
                    process.StandardInput.WriteLine(hcl);
                    process.StandardInput.Close();

                    Task.WhenAll(ForwardLogs(process.StandardError, $"{c.Application.Name}:{c.Name}:nomad:stderr"));

                    process.WaitForExit();
                    _logger.LogTrace($"nomad job {c.Application.Name}-{c.Name} is ready");
                }
            }
            catch (Win32Exception e)  // yes, even on Linux
            {
                if (e.Message.Contains("No such file or directory", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("The system cannot find the file specified", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogError("nomad command not found in PATH");
                    return;
                }
                throw;
            }
        }

        public override void Stop(Channel c)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "nomad",
                Arguments = $"job stop {c.Application.Name}-{c.Name}",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
            Process.Start(psi);
        }

        private static string JobDefinition(Channel c)
        {
            var name = $"{c.Application.Name}-{c.Name}";
            var bindle = $"{c.Application.StorageId}/{c.ActiveRevision.RevisionNumber}";
            var env = String.Join(' ', c.GetEnvironmentVariables().Select(ev => $"\"--env\", \"{ev.Key}='{ev.Value}'\","));

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


        // TODO: deduplicate without incurring a check on every line

        private async Task ForwardLogs(StreamReader source, string streamId)
        {
            string line;
            while ((line = await source.ReadLineAsync()) != null)
            {
                _logger.LogTrace($"{streamId}: {line}");
            }
        }
    }
}
