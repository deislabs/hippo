using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Hippo.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public class WagiLocalJobScheduler : IJobScheduler
    {
        // This assumes a singleton scheduler instance!
        private readonly Dictionary<Guid, int> _wagiProcessIds = new();
        private readonly ILogger _logger;

        private const string ENV_BINDLE = "BINDLE_SERVER_URL";

        private const string ENV_WAGI = "HIPPO_WAGI_PATH";


        public WagiLocalJobScheduler(IHostApplicationLifetime lifetime, ILogger<WagiLocalJobScheduler> logger)
        {
            _logger = logger;

            var bindleUrl = Environment.GetEnvironmentVariable(ENV_BINDLE);

            if (string.IsNullOrWhiteSpace(bindleUrl))
            {
                _logger.LogError($"Bindle server URL not specified: set {ENV_BINDLE}");
                _logger.LogCritical($"No channels will be able to run - this scheduler requires {ENV_BINDLE}");
            }

            lifetime.ApplicationStopping.Register(() =>
            {
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
            var bindleUrl = Environment.GetEnvironmentVariable(ENV_BINDLE);

            if (string.IsNullOrWhiteSpace(bindleUrl))
            {
                _logger.LogError($"Bindle server URL not specified: set {ENV_BINDLE}");
                return;
            }

            var psi = new ProcessStartInfo
            {
                FileName = wagiProgram,
                Arguments = $"-b {c.Application.StorageId}/{c.ActiveRevision.RevisionNumber} --bindle-server {bindleUrl} --default-host localhost:{port} -l 127.0.0.1:{port}",
            };
            psi.Environment["BINDLE_SERVER_URL"] = bindleUrl;
            // TODO: drive this from outside
            psi.Environment["RUST_LOG"] = "warn,wagi=trace";

            try
            {
                using (var process = Process.Start(psi))
                {
                    process.Exited += (s, e) => _wagiProcessIds.Remove(c.Id);
                    _wagiProcessIds[c.Id] = process.Id;
                }
            }
            catch (Win32Exception e)  // yes, even on Linux
            {
                if (e.Message.Contains("No such file or directory"))
                {
                    _logger.LogError($"Program '{wagiProgram}' not found: check system path or set {ENV_WAGI}");
                    return;
                }
                throw;
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
            return Environment.GetEnvironmentVariable(ENV_WAGI) ??
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
