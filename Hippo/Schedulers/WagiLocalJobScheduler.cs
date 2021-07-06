using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.Providers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public class WagiLocalJobScheduler : BaseScheduler
    {
        // This assumes a singleton scheduler instance!
        private readonly Dictionary<Guid, int> _wagiProcessIds = new();

        private const string ENV_BINDLE = "BINDLE_URL";

        private const string ENV_WAGI = "HIPPO_WAGI_PATH";


        public WagiLocalJobScheduler(IHostApplicationLifetime lifetime, ILogger<WagiLocalJobScheduler> logger, IProxyConfigUpdater proxyConfigUpdater)
            : base(logger, proxyConfigUpdater)
        {
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
                    var (id, log) = processId.Value;
                    KillProcessById(id);
                    log.GetAwaiter().GetResult();
                }
            });
        }

        public override void Start(Channel c)
        {
            var port = c.PortID + Channel.EphemeralPortRange;
            var wagiProgram = WagiBinaryPath();
            var bindleUrl = Environment.GetEnvironmentVariable(ENV_BINDLE);
            var listenAddress = $"127.0.0.1:{port}";

            if (string.IsNullOrWhiteSpace(bindleUrl))
            {
                _logger.LogError($"Bindle server URL not specified: set {ENV_BINDLE}");
                return;
            }

            var env = String.Join(' ', c.GetEnvironmentVariables().Select(ev => $"--env {ev.Key}=\"{ev.Value}\""));

            var psi = new ProcessStartInfo
            {
                FileName = wagiProgram,
                Arguments = $"-b {c.Application.StorageId}/{c.ActiveRevision.RevisionNumber} --bindle-server {bindleUrl} -l 127.0.0.1:{port} {env}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            psi.Environment["BINDLE_URL"] = bindleUrl;
            // TODO: drive this from outside
            psi.Environment["RUST_LOG"] = "warn,wagi=trace";

            try
            {
                // TODO: There is a problem here in that the process object goes out of scope at the end of the function:
                // There are at least 2 problems with this:
                // 
                // There is a race condition, sometimes the Exited event does not fire, this means that Yarpconfigs can stay around if the process immediately exits.
                // I think that the output from the process gets lost once the process goes out of scope.
                // We cannot detect reliably if the process immediately exits (which could be an indicator of a bad command line where there is a new version of wagi that has changed 
                // arguments and/or environment variables. 
                // 
                // It also makes it hard/error prone to redirect the stdout and stderr to ILogger.
                // May be better to use a Task for each process and have the task live for as long as the process is running by using process.WaitForExit()
                // Can then kill the task/process with a cancellation token.

                using var process = new Process();
                process.EnableRaisingEvents = true;
                process.StartInfo = psi;
                process.Exited += (s, e) =>
                {
                    _wagiProcessIds.Remove(c.Id);
                    RemoveChannelFromWarpConfig(c);
                };
                process.Start();

                if (!process.HasExited)
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += (s, e) => _wagiProcessIds.Remove(c.Id);
                    var log = Task.WhenAll(
                        ForwardLogs(process.StandardOutput, $"{c.Application.Name}:{c.Name}:wagi:stdout", LogLevel.Trace),
                        ForwardLogs(process.StandardError, $"{c.Application.Name}:{c.Name}:wagi:stderr")
                    );
                    _wagiProcessIds[c.Id] = (process.Id, log);
                }
            }
            catch (Win32Exception e)  // yes, even on Linux
            {
                if (e.Message.Contains("No such file or directory", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogError($"Program '{wagiProgram}' not found: check system path or set {ENV_WAGI}");
                    return;
                }
                throw;
            }
        }

        public override void Stop(Channel c)
        {
            if (_wagiProcessIds.TryGetValue(c.Id, out var wagiProcess))
            {
                _wagiProcessIds.Remove(c.Id);

                var (wagiProcessId, log) = wagiProcess;
                KillProcessById(wagiProcessId);
                log.GetAwaiter().GetResult();
            }
        }

        // TODO: deduplicate without incurring a check on every line

        private async Task ForwardLogs(StreamReader source, string streamId)
        {
            string line;
            while ((line = await source.ReadLineAsync()) != null)
            {
                _logger.Log(ConvertLogLevel(line), $"{streamId}: {line}");
            }
        }

        private async Task ForwardLogs(StreamReader source, string streamId, LogLevel logLevel)
        {
            string line;
            while ((line = await source.ReadLineAsync()) != null)
            {
                _logger.Log(logLevel, $"{streamId}: {line}");
            }
        }

        private static readonly IReadOnlyList<string> RUST_TRACE_LEVELS = new List<string> {
            "TRACE", "DEBUG", "INFO", "WARN", "ERROR" // TODO: check
        }.AsReadOnly();

        private static LogLevel ConvertLogLevel(string rustTraceLine)
        {
            var rustLevel = ExtractRustTraceLevel(rustTraceLine);
            return rustLevel switch
            {
                "TRACE" => LogLevel.Trace,
                "DEBUG" => LogLevel.Debug,
                "INFO" => LogLevel.Information,
                "WARN" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                _ => LogLevel.Warning,
            };
        }

        private static string ExtractRustTraceLevel(string rustTraceLine) =>
            rustTraceLine.Split(' ').Select(AsRustTraceLevel).FirstOrDefault(s => s != null);

        private static string AsRustTraceLevel(string fragment) =>
            RUST_TRACE_LEVELS.Where(level => fragment.Contains(level, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

        private static string WagiBinaryPath()
        {
            return Environment.GetEnvironmentVariable(ENV_WAGI) ??
                (OperatingSystem.IsWindows() ? "wagi.exe" : "wagi");
        }

        private static void KillProcessById(int wagiProcessId)
        {
            try
            {
                using var wagiProcess = Process.GetProcessById(wagiProcessId);
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
            catch
            {
                // TODO: process not running: log and move on
            }
        }
    }
}
