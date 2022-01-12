using System.ComponentModel;
using System.Diagnostics;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Common;
using Hippo.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Infrastructure.JobSchedulers;

public class LocalJobScheduler : IJobScheduler, IHasDomainEvent
{
    public const int EphemeralPortStartRange = 32768;

    private readonly ILogger<LocalJobScheduler> _logger;

    private readonly IConfiguration _configuration;

    private readonly string _bindleUrl;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

    // This assumes a singleton scheduler instance!
    private readonly Dictionary<Guid, (int, Task)> _wagiProcessIds = new();

    public LocalJobScheduler(ILogger<LocalJobScheduler> logger, IConfiguration configuration, IHostApplicationLifetime lifetime)
    {
        _logger = logger;

        _configuration = configuration;

        _bindleUrl = _configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1");
        if (_bindleUrl == null)
        {
            throw new ArgumentException("Bindle URL cannot be null.");
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

    public ChannelStatus Start(Channel c)
    {
        var port = c.PortId + EphemeralPortStartRange;
        var wagiProgram = WagiBinaryPath();
        var listenAddress = $"127.0.0.1:{port}";

        if (c.ActiveRevision == null || string.IsNullOrWhiteSpace(c.ActiveRevision.RevisionNumber))
        {
            _logger.LogWarning($"Can't start {c.App.Name}:{c.Name}: no active revision");
            return new ChannelStatus(false, "");
        }

        var env = String.Join(' ', c.EnvironmentVariables.Select(ev => $"--env {ev.Key}=\"{ev.Value}\""));

        var psi = new ProcessStartInfo
        {
            FileName = wagiProgram,
            Arguments = $"-b {c.App.StorageId}/{c.ActiveRevision.RevisionNumber} --bindle-url {_bindleUrl} -l 127.0.0.1:{port} {env}",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        psi.Environment["BINDLE_URL"] = _bindleUrl;
        // TODO: drive this from outside
        psi.Environment["RUST_LOG"] = "warn,wagi=trace";

        try
        {
            using (var process = Process.Start(psi))
            {
                if (process == null)
                {
                    // TODO: probably want to throw an Exception here instead
                    _logger.LogError($"Process {psi.FileName} with arguments {psi.Arguments} never started");
                    return new ChannelStatus(false, "");
                }

                process.EnableRaisingEvents = true;
                // TODO: this event handler does not always fire, if the program immediately exits (for example because the command line is wrong because an old version
                // of wagi is being used then the process object may go out of scope before the event handler fires.
                // Should probably capture the process not just the Id in the dictionary and then the issue will be resolved.
                process.Exited += (s, e) =>
                {
                    _wagiProcessIds.Remove(c.Id);
                };
                process.Start();
                var log = Task.WhenAll(
                        ForwardLogs(process.StandardOutput, $"{c.App.Name}:{c.Name}:wagi:stdout", LogLevel.Trace),
                        ForwardLogs(process.StandardError, $"{c.App.Name}:{c.Name}:wagi:stderr")
                        );
                if (process.HasExited)
                {
                    _logger.LogError($"Process {psi.FileName} with arguments {psi.Arguments} terminated unexpectedly");
                    return new ChannelStatus(false, "");
                }
                _wagiProcessIds[c.Id] = (process.Id, log);
                return new ChannelStatus(true, listenAddress);
            }
        }
        catch (Win32Exception e)  // yes, even on Linux
        {
            if (e.Message.Contains("No such file or directory", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("The system cannot find the file specified", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException($"The system cannot find '{wagiProgram}'; add '{wagiProgram}' to your $PATH or set 'Wagi:BinaryPath' in your appsettings to the correct location.");
            }
            throw;
        }
    }

    public void Stop(Channel c)
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
        string? line;
        while ((line = await source.ReadLineAsync()) != null)
        {
            _logger.Log(ConvertLogLevel(line), $"{streamId}: {line}");
        }
    }

    private async Task ForwardLogs(StreamReader source, string streamId, LogLevel logLevel)
    {
        string? line;
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

    private static string? ExtractRustTraceLevel(string rustTraceLine) =>
        rustTraceLine.Split(' ').Select(AsRustTraceLevel).FirstOrDefault(s => s != null);

    private static string? AsRustTraceLevel(string fragment) =>
        RUST_TRACE_LEVELS.Where(level => fragment.Contains(level, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

    private string WagiBinaryPath()
    {
        return _configuration.GetValue<string>("Wagi:BinaryPath", (OperatingSystem.IsWindows() ? "wagi.exe" : "wagi"));
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
