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
        public void UnPublish()
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
        /// Publish the current release with no changes to the "auto deploy"
        /// feature.
        /// </summary>
        public void Publish()
        {
            File.WriteAllText(WagiConfigPath(), Toml.Parse(WagiConfig()).ToString());
            File.WriteAllText(SystemdServicePath(), SystemdService());
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
            File.WriteAllText(TraefikConfigPath(), Toml.Parse(TraefikConfig()).ToString());
        }

        /// <summary>
        /// Publish the current release. If autoDeploy is set to false, the
        /// channel will no longer automatically deploy the next available
        /// release.
        /// </summary>
        public void Publish(bool autoDeploy)
        {
            AutoDeploy = autoDeploy;
            Publish();
        }

        public string TraefikConfig()
        {
            if (Domain == null)
            {
                return "";
            }
            var routers = new Dictionary<string, object>();
            var services = new Dictionary<string, object>();
            var traefikConfig = new { Http = new { Routers = routers, Services = services}};
            // start from the ephemeral port range
            var port = Port + EphemeralPortRange;
            routers.Add(
                String.Format("to-{0}-{1}", Application.Name, Name),
                new Dictionary<string, string>
                {
                    {
                        "rule", String.Format("Host(`{0}`) && PathPrefix(`/`)", Domain.Name)
                    },
                    {
                        "service", String.Format("{0}-{1}", Application.Name, Name)
                    }
                }
            );
            services.Add(
                String.Format("{0}-{1}", Application.Name, Name),
                new Dictionary<string, object>
                {
                    {
                        "LoadBalancer",
                        new Dictionary<string, object>
                        {
                            {
                                "servers",
                                new List<Dictionary<string, string>>
                                {
                                    new Dictionary<string, string>
                                    {
                                        {
                                            "url", String.Format("http://localhost:{0}", port)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            );
            return JsonSerializer.Serialize(traefikConfig, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
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
            return Path.Combine("/etc", "systemd", "system", "hippo-" + Name + ".service");
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
