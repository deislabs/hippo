using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Jobs;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Jobs;

public class LocalJob : Job
{
    private static readonly IPEndPoint DefaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, port: 0);
    public string BindleId;
    private readonly Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
    private readonly string bindleUrl;
    private readonly string spinBinaryPath;
    private Process? process;

    public LocalJob(IConfiguration configuration, Guid id, string bindleId) : base(id)
    {
        BindleId = bindleId;
        bindleUrl = configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1");
        spinBinaryPath = configuration.GetValue<string>("Spin:BinaryPath", (OperatingSystem.IsWindows() ? "spin.exe" : "spin"));
    }

    public void AddEnvironmentVariable(string key, string value)
    {
        environmentVariables[key] = value;
    }

    public override void Run()
    {
        try
        {
            process = Process.Start(psi());
            if (process is null)
            {
                throw new JobFailedException("Job never started");
            }

            process.Start();
            _status = JobStatus.Running;

            // kill the process if the calling thread cancels the Job.
            cancellationToken.Register(() =>
            {
                process.Kill();
                _status = JobStatus.Canceled;
            });
        }
        catch (Win32Exception e)  // yes, even on Linux
        {
            if (e.Message.Contains("No such file or directory", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("The system cannot find the file specified", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException($"The system cannot find '{spinBinaryPath}'; add '{spinBinaryPath}' to your $PATH or set 'Nomad:BinaryPath' in your appsettings to the correct location.");
            }
            throw;
        }
    }

    public override void Stop()
    {
        // no need to tell spin to stop if the process hasn't started
        if (IsWaitingToRun || IsCompleted)
        {
            return;
        }

        process?.Kill();
        _status = JobStatus.Completed;
    }

    public override void Reload()
    {
        if (IsRunning)
        {
            // TODO: restart process, re-binding to the same port number
            process?.Kill();
            Run();
        }
    }

    private ProcessStartInfo psi()
    {
        var env = String.Join(' ', environmentVariables.Select(ev => $"--env {ev.Key}=\"{ev.Value}\""));

        return new ProcessStartInfo
        {
            FileName = spinBinaryPath,
            Arguments = $"up --bindle {BindleId} --server {bindleUrl} --listen 127.0.0.1:{GetAvailablePort()} {env}",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
    }

    private static int GetAvailablePort()
    {
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            socket.Bind(DefaultLoopbackEndpoint);
            return ((IPEndPoint)socket.LocalEndPoint!).Port;
        }
    }
}
