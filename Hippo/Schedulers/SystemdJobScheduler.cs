using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Hippo.Models;
using Nett;

namespace Hippo.Schedulers
{
    public class SystemdJobScheduler : IJobScheduler
    {
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
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "systemctl",
                    Arguments = "start " + SystemdServicePath(c),
                }
            };
            process.Start();
            process.WaitForExit();
            FileInfo traefikConfigFile = new(TraefikConfigPath(c));
            traefikConfigFile.Directory.Create();
            File.WriteAllText(traefikConfigFile.FullName, TraefikConfig(c));
        }

        public void Stop(Channel c)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "systemctl",
                    Arguments = "stop " + SystemdServicePath(c),
                }
            };
            process.Start();
            process.WaitForExit();
        }

        public static string TraefikConfig(Channel c)
        {
            if (c.Domain == null)
            {
                return "";
            }

            // start from the ephemeral port range
            var port = c.PortID + Channel.EphemeralPortRange;
            var serviceId = $"{c.Application.Name}-{c.Name}";

            var routers = new Dictionary<string, object> {
                {
                    $"to-{serviceId}",
                    new {
                        rule = $"Host(`{c.Domain.Name}`) && PathPrefix(`/`)",
                        service = serviceId
                    }
                }
            };
            var services = new Dictionary<string, object> {
                {
                    serviceId,
                    new {
                        loadBalancer = new {
                            servers = new [] {
                                new { url = $"http://localhost:{port}" }
                            }
                        }
                    }
                }
            };

            var traefikConfig = new { http = new { routers, services } };
            return Toml.WriteString(traefikConfig);
        }

        public static string TraefikConfigPath(Channel c)
        {
            return Path.Combine("/etc", "traefik", "conf.d", c.Name + ".toml");
        }

        public static string SystemdService(Channel c)
        {
            var systemdService = new StringBuilder();
            systemdService.AppendLine("[Unit]");
            systemdService.AppendFormat(CultureInfo.CurrentCulture, "Description=Hippo runtime for app {0} channel {1}\n", c.Application.Name, c.Name);
            systemdService.AppendLine();
            systemdService.AppendLine("[Service]");
            systemdService.AppendLine("Type=simple");
            // TODO: make wagi system path configurable
            systemdService.AppendFormat(CultureInfo.CurrentCulture, "ExecStart=/usr/local/bin/wagi --config {0} --listen 0.0.0.0:{1}\n", WagiConfigPath(c), c.PortID + Channel.EphemeralPortRange);
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
            wagiConfig.AppendFormat(CultureInfo.CurrentCulture, "module = \"bindle:{0}/{1}\"\n", c.Application.StorageId, c.ActiveRevision.RevisionNumber);
            foreach (EnvironmentVariable envvar in c.Configuration.EnvironmentVariables)
            {
                wagiConfig.AppendFormat(CultureInfo.CurrentCulture, "environment.{0} = \"{1}\"\n", envvar.Key, envvar.Value);
            }
            return wagiConfig.ToString();
        }

        public static string WagiConfigPath(Channel c)
        {
            return Path.Combine("/etc", "wagi", c.Id.ToString(), "modules.toml");
        }
    }
}
