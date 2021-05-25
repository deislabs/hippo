using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Hippo.Models;

namespace Hippo.Schedulers
{
    public class WagiLocalJobScheduler: IJobScheduler
    {
        // This assumes a singleton scheduler instance!
        private readonly Dictionary<Guid, int> _wagiProcessIds = new();
        
        public void Start(Channel c)
        {
            FileInfo wagiConfigFile = new(WagiConfigPath(c));
            wagiConfigFile.Directory.Create();
            File.WriteAllText(wagiConfigFile.FullName, WagiConfig(c));

            var port = c.PortID + Channel.EphemeralPortRange;

            // TODO: ProcessStartInfo.CreateNoWindow (but want to keep as a child process)
            // TODO: On Windows do we need an OS job object to auto exit child processes when
            // parent exits?  (On Unixalikes I *think* we get this free.)
            using (var process = Process.Start("wagi", $"-c {wagiConfigFile.FullName} -l 127.0.0.1:{port}"))
            {
                _wagiProcessIds[c.Id] = process.Id;
            }
        }

        public void Stop(Channel c)
        {
            if (_wagiProcessIds.TryGetValue(c.Id, out var wagiProcessId))
            {
                _wagiProcessIds.Remove(c.Id);
                using (var wagiProcess = Process.GetProcessById(wagiProcessId))
                {
                    if (wagiProcess != null)
                    {
                        wagiProcess.Kill(true); // I don't think there's a less awful way to do this
                        wagiProcess.WaitForExit();
                    }
                }
            }
        }

        // TODO: deduplicate with SystemdJobScheduler
        public static string WagiConfig(Channel c)
        {
            var wagiConfig = new StringBuilder();
            wagiConfig.AppendLine("[[module]]");
            wagiConfig.AppendFormat("module = \"{0}\"\n", c.Release.UploadUrl.ToString());
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
