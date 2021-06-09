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
                foreach (var processId in _wagiProcessIds)
                {
                    KillProcessById(processId.Value);
                }
            });
        }

        public void OnSchedulerStart(IEnumerable<Application> applications)
        {
            foreach (var application in applications)
            {
                foreach (var channel in application.Channels)
                {
                    Start(channel);
                }
            }
        }
        
        public void Start(Channel c)
        {
            var port = c.PortID + Channel.EphemeralPortRange;

            var wagiProgram = WagiBinaryPath();
            var psi = new ProcessStartInfo
            {
                FileName = wagiProgram,
                Arguments = $"-b {c.Application.StorageId}/{c.ActiveRevision.RevisionNumber} --bindle-server {Environment.GetEnvironmentVariable("BINDLE_SERVER_URL")} --default-host localhost:{port} -l 127.0.0.1:{port}",
            };
            psi.Environment["BINDLE_SERVER_URL"] = Environment.GetEnvironmentVariable("BINDLE_SERVER_URL");
            // TODO: drive this from outside
            psi.Environment["RUST_LOG"] = "warn,wagi=trace";

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

        private static string WagiBinaryPath()
        {
            return Environment.GetEnvironmentVariable("HIPPO_WAGI_PATH") ??
                (OperatingSystem.IsWindows() ? "wagi.exe" : "wagi");
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
    }
}
