using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Tomlyn;

namespace Hippo.Models
{
    public class Channel: BaseEntity
    {
        private const int EphemeralPortRange = 32768;

        public string Name { get; set; }

        public bool AutoDeploy { get; set; }

        public string VersionRange { get; set; }
        public Application Application { get; set; }
        public Domain Domain { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Port { get; set; }
        public Configuration Configuration { get; set; }
        public Release Release { get; set; }

        /// <summary>
        /// Gracefully shut down the current release. This prevents the channel
        /// from receiving requests.
        /// </summary>
        public void Stop()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "systemctl",
                    Arguments = "stop " + SystemdServicePath(),
                }
            };
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Start the current release with no changes to the "auto deploy"
        /// feature.
        /// </summary>
        public void Start()
        {
            FileInfo wagiConfigFile = new(WagiConfigPath());
            wagiConfigFile.Directory.Create();
            File.WriteAllText(wagiConfigFile.FullName, Toml.Parse(WagiConfig()).ToString());
            FileInfo systemdServiceFile = new(SystemdServicePath());
            systemdServiceFile.Directory.Create();
            File.WriteAllText(systemdServiceFile.FullName, SystemdService());
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "systemctl",
                    Arguments = "start " + SystemdServicePath(),
                }
            };
            process.Start();
            process.WaitForExit();
            FileInfo traefikConfigFile = new(TraefikConfigPath());
            traefikConfigFile.Directory.Create();
            File.WriteAllText(traefikConfigFile.FullName, Toml.Parse(TraefikConfig()).ToString());
        }

        /// <summary>
        /// Start the current release. If autoDeploy is set to false, the
        /// channel will no longer automatically deploy the next available
        /// release.
        /// </summary>
        public void Start(bool autoDeploy)
        {
            AutoDeploy = autoDeploy;
            Start();
        }

        public string TraefikConfig()
        {
            if (Domain == null)
            {
                return "";
            }

            // start from the ephemeral port range
            var port = Port + EphemeralPortRange;
            var serviceId = $"{Application.Name}-{Name}";

            var routers = new Dictionary<string, object> {
                {
                    $"to-{serviceId}",
                    new {
                        rule = $"Host(`{Domain.Name}`) && PathPrefix(`/`)",
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

            var traefikConfig = new { http = new { routers, services}};
            return Nett.Toml.WriteString(traefikConfig);
        }

        public string TraefikConfigPath()
        {
            return Path.Combine("/etc", "traefik", "conf.d", Name + ".toml");
        }

        public string SystemdService()
        {
            var systemdService = new StringBuilder();
            systemdService.AppendLine("[Unit]");
            systemdService.AppendFormat("Description=Hippo runtime for app {0} channel {1}\n", Application.Name, Name);
            systemdService.AppendLine();
            systemdService.AppendLine("[Service]");
            systemdService.AppendLine("Type=simple");
            // TODO: make wagi system path configurable
            systemdService.AppendFormat("ExecStart=/usr/local/bin/wagi --config {0} --listen 0.0.0.0:{1}\n", WagiConfigPath(), Port + EphemeralPortRange);
            systemdService.AppendLine();
            systemdService.AppendLine("[Install]");
            systemdService.AppendLine("WantedBy=multi-user.target");
            return systemdService.ToString();
        }

        public string SystemdServicePath()
        {
            return Path.Combine("/etc", "systemd", "system", "hippo-" + Application.Name + "-" + Name + ".service");
        }

        public string WagiConfig()
        {
            var wagiConfig = new StringBuilder();
            wagiConfig.AppendLine("[[module]]");
            wagiConfig.AppendFormat("module = \"{0}\"\n", Release.UploadUrl.ToString());
            foreach (EnvironmentVariable envvar in Configuration.EnvironmentVariables)
            {
                wagiConfig.AppendFormat("environment.{0} = \"{1}\"\n", envvar.Key, envvar.Value);
            }
            return wagiConfig.ToString();
        }

        public string WagiConfigPath()
        {
            return Path.Combine("/etc", "wagi", Id.ToString(), "modules.toml");
        }
    }
}
