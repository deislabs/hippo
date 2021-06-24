using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Hippo.Models;
using Hippo.Services;
using Nett;

namespace Hippo.Schedulers
{
    public class SystemdJobScheduler : IJobScheduler
    {
        private readonly ITraefikService _traefikService;

        public SystemdJobScheduler(ITraefikService traefikService)
        {
            _traefikService = traefikService;
        }

        public void OnSchedulerStart(IEnumerable<Application> applications)
        {
            // Nothing to do - apps run independently of scheduler object lifecycle
        }

        public void Start(Channel c)
        {
            FileInfo wagiConfigFile = new(WagiConfigPath(c));
            wagiConfigFile.Directory.Create();
            File.WriteAllText(wagiConfigFile.FullName, WagiConfig(c));
            FileInfo systemdServiceFile = new(SystemdServicePath(c));
            systemdServiceFile.Directory.Create();
            File.WriteAllText(systemdServiceFile.FullName, SystemdService(c));
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "systemctl",
                    Arguments = "start " + SystemdServicePath(c),
                }
            };
            process.Start();
            process.WaitForExit();

            // start from the ephemeral port range
            var port = c.PortID + Channel.EphemeralPortRange;
            _traefikService.StartProxy(c.UniqueName(), c.Domain.Name, $"http://localhost:{port}");
        }

        public void Stop(Channel c)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "systemctl",
                    Arguments = "stop " + SystemdServicePath(c),
                }
            };
            process.Start();
            process.WaitForExit();
            _traefikService.StopProxy(c.UniqueName());
        }

        public static string SystemdService(Channel c)
        {
            var systemdService = new StringBuilder();
            systemdService.AppendLine("[Unit]");
            systemdService.AppendFormat("Description=Hippo runtime for app {0} channel {1}\n", c.Application.Name, c.Name);
            systemdService.AppendLine();
            systemdService.AppendLine("[Service]");
            systemdService.AppendLine("Type=simple");
            // TODO: make wagi system path configurable
            systemdService.AppendFormat("ExecStart=/usr/local/bin/wagi --config {0} --listen 0.0.0.0:{1}\n", WagiConfigPath(c), c.PortID + Channel.EphemeralPortRange);
            systemdService.AppendLine();
            systemdService.AppendLine("[Install]");
            systemdService.AppendLine("WantedBy=multi-user.target");
            return systemdService.ToString();
        }

        public static string SystemdServicePath(Channel c)
        {
            return Path.Combine("/etc", "systemd", "system", "hippo-" + c.Application.Name + "-" + c.Name + ".service");
        }

        public static string WagiConfig(Channel c)
        {
            var wagiConfig = new StringBuilder();
            wagiConfig.AppendLine("[[module]]");
            wagiConfig.AppendFormat("module = \"bindle:{0}/{1}\"\n", c.Application.StorageId, c.ActiveRevision.RevisionNumber);
            foreach (EnvironmentVariable envvar in c.Configuration.EnvironmentVariables)
            {
                wagiConfig.AppendFormat("environment.{0} = \"{1}\"\n", envvar.Key, envvar.Value);
            }
            return wagiConfig.ToString();
        }

        public static string WagiConfigPath(Channel c)
        {
            return Path.Combine("/etc", "wagi", c.Id.ToString(), "modules.toml");
        }
    }
}
