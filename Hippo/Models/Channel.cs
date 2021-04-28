using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Tomlyn;

namespace Hippo.Models
{
    public class Channel: BaseEntity
    {
        public string Name { get; set; }
        public Application Application { get; set; }
        public Domain Domain { get; set; }
        public Configuration Configuration { get; set; }
        public Release Release { get; set; }

        public void Publish()
        {
            File.WriteAllText(WagiConfigPath(), Toml.Parse(WagiConfig()).ToString());
            File.WriteAllText(SystemdServicePath(), SystemdService());
            // TODO: start the systemd service before writing out the traefik config
            // https://github.com/deislabs/hippo/blob/e0a5ed97cd1b00ec93fb3515ed51c3c5b9ee02d0/releases/models.py#L34-L41
            // https://seshuk.com/2020-06-02-linux-exec-dotnetcore/
            File.WriteAllText(TraefikConfigPath(), Toml.Parse(TraefikConfig()).ToString());
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
            // TODO: determine port number from systemd
            // https://github.com/deislabs/hippo/blob/e0a5ed97cd1b00ec93fb3515ed51c3c5b9ee02d0/releases/models.py#L97-L111
            // var pid = 0;
            var port = 8080;
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
            systemdService.AppendFormat("ExecStart=/usr/local/bin/wagi --config {0} --listen 0.0.0.0:0\n", WagiConfigPath());
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
