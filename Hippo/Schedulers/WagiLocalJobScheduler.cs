using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Hippo.Models;
using Microsoft.Extensions.Hosting;

namespace Hippo.Schedulers
{
    public class WagiLocalJobScheduler: IJobScheduler
    {
        // This assumes a singleton scheduler instance!
        private readonly Dictionary<Guid, int> _wagiProcessIds = new();

        public WagiLocalJobScheduler(IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStopping.Register(() => {
                foreach (var processId in _wagiProcessIds) {
                    KillProcessById(processId.Value);
                }
            });
        }
        
        public void Start(Channel c)
        {
            FileInfo wagiConfigFile = new(WagiConfigPath(c));
            wagiConfigFile.Directory.Create();
            File.WriteAllText(wagiConfigFile.FullName, WagiConfig(c));

            var port = c.PortID + Channel.EphemeralPortRange;

            var wagiProgram = OperatingSystem.IsWindows() ? "wagi.exe" : "/home/ivan/github/wagi/target/debug/wagi";
            var psi = new ProcessStartInfo
            {
                FileName = wagiProgram,
                Arguments = $"-c {wagiConfigFile.FullName} -l 127.0.0.1:{port}",
            };
            psi.Environment["BINDLE_SERVER_URL"] = Environment.GetEnvironmentVariable("BINDLE_SERVER_URL");
            psi.Environment["RUST_LOG"] = "warn,wagi=trace";
            Console.WriteLine(psi.Environment["BINDLE_SERVER_URL"]);
            using (var process = Process.Start(psi))
            {
                process.Exited += (s, e) => _wagiProcessIds.Remove(c.Id);
                _wagiProcessIds[c.Id] = process.Id;
            }
        }

        public void Stop(Channel c)
        {
            if (_wagiProcessIds.TryGetValue(c.Id, out var wagiProcessId))
            {
                _wagiProcessIds.Remove(c.Id);
                KillProcessById(wagiProcessId);
            }
        }

        private static void KillProcessById(int wagiProcessId)
        {
            try
            {
                using (var wagiProcess = Process.GetProcessById(wagiProcessId))
                {
                    if (wagiProcess != null && !wagiProcess.HasExited)
                    {
                        // TODO: check it is an actual wagi process and not something that reused the ID
                        // TODO: a better way to do this might be to hang onto the Process object not
                        // just the ID
                        try
                        {
                            wagiProcess.Kill(true); // I don't think there's a less awful way to do this
                            wagiProcess.WaitForExit();
                        }
                        catch
                        {
                            // TODO: log it and move on
                        }
                    }
                }
            }
            catch
            {
                // TODO: process not running: log and move on
            }
        }

        // TODO: deduplicate with SystemdJobScheduler
        public static string WagiConfig(Channel c)
        {
            var wagiConfig = new StringBuilder();
            wagiConfig.AppendLine("[[module]]");
            wagiConfig.AppendFormat("module = \"{0}\"\n", c.Release.UploadUrl.ToString());
            var bindleServer = Environment.GetEnvironmentVariable("BINDLE_SERVER_URL");
            if (bindleServer != null)
            {
                wagiConfig.AppendFormat("bindle_server = \"{0}\"\n", bindleServer);
            }
            wagiConfig.AppendLine("route = \"/\"");
            foreach (EnvironmentVariable envvar in c.Configuration.EnvironmentVariables)
            {
                wagiConfig.AppendFormat("environment.{0} = \"{1}\"\n", envvar.Key, envvar.Value);
            }
            return wagiConfig.ToString();
        }

        public static string WagiConfigPath(Channel c)
        {
            return Path.Combine("/tmp", "wagi", c.Id.ToString(), "modules.toml");
        }
    }
}
