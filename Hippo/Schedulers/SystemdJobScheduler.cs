using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Hippo.Models;
using Nett;

namespace Hippo.Schedulers
{
    public class SystemdJobScheduler : IJobScheduler
    {
        // TODO: make this configurable
        private PortMapper _portMapper = new(PortMapper.EphemeralPortStartRange, PortMapper.MaxPortNumber);

        private Dictionary<Guid, int> _portMappings = new();

        public void OnSchedulerStart(IEnumerable<Application> applications)
        {
            // Nothing to do - apps run independently of scheduler object lifecycle
            // TODO: we could populate _portMapper with any existing systemd jobs
        }

        public void Start(Channel c)
        {
            int port = _portMapper.ReservePort();
            FileInfo wagiConfigFile = new(WagiConfigPath(c));
            wagiConfigFile.Directory.Create();
            File.WriteAllText(wagiConfigFile.FullName, WagiConfig(c));
            FileInfo systemdServiceFile = new(SystemdServicePath(c));
            systemdServiceFile.Directory.Create();
            File.WriteAllText(systemdServiceFile.FullName, SystemdService(c, port));
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
            FileInfo traefikConfigFile = new(TraefikConfigPath(c));
            traefikConfigFile.Directory.Create();
            File.WriteAllText(traefikConfigFile.FullName, TraefikConfig(c, port));
            _portMappings.Add(c.Id, port);
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
            if (_portMappings.TryGetValue(c.Id, out var port))
            {
                _portMappings.Remove(c.Id);
                _portMapper.FreePort(port);
            }
        }

        public static string TraefikConfig(Channel c, int port)
        {
            if (c.Domain == null)
            {
                return "";
            }

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

        public static string SystemdService(Channel c, int port)
        {
            var systemdService = new StringBuilder();
            systemdService.AppendLine("[Unit]");
            systemdService.AppendFormat("Description=Hippo runtime for app {0} channel {1}\n", c.Application.Name, c.Name);
            systemdService.AppendLine();
            systemdService.AppendLine("[Service]");
            systemdService.AppendLine("Type=simple");
            // TODO: make wagi system path configurable
            systemdService.AppendFormat("ExecStart=/usr/local/bin/wagi --config {0} --listen 0.0.0.0:{1}\n", WagiConfigPath(c), port);
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

        public ChannelStatus Status(Channel c)
        {
            ChannelStatus status = new()
            {
                IsRunning = false
            };
            if (_portMappings.TryGetValue(c.Id, out var port))
            {
                status.IsRunning = true;
                status.Port = port;
            }
            return status;
        }
    }
}
