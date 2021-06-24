using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nett;

namespace Hippo.Services
{
    public class TraefikService : ITraefikService, IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly string _binPath;
        private readonly string _configDirectory;
        private readonly string _listenAddress;
        private readonly ILogger<TraefikService> _logger;

        public TraefikService(ILogger<TraefikService> logger, IHostEnvironment hostingEnvironment)
        {
            // TODO(bacongobbler): make this configurable
            _binPath = "traefik";
            // TODO(bacongobbler): make this configurable
            _configDirectory = Path.Combine(hostingEnvironment.ContentRootPath, "etc", "traefik");
            // TODO(bacongobbler): make this configurable. We probably want traefik serving on port 80 for production environments. But that requries sudo.
            _listenAddress = "0.0.0.0:32780";
            _logger = logger;
        }

        public void StartProxy(string name, string hostname, string proxyUrl)
        {
            FileInfo traefikConfigFile = new(Path.Combine(_configDirectory, $"{name}.toml"));

            var routers = new Dictionary<string, object> {
                {
                    $"to-{name}",
                    new {
                        rule = $"Host(`{hostname.ToString()}`) && PathPrefix(`/`)",
                        service = name
                    }
                }
            };
            var services = new Dictionary<string, object> {
                {
                    name,
                    new {
                        loadBalancer = new {
                            servers = new [] {
                                new { url = $"{proxyUrl.ToString()}" }
                            }
                        }
                    }
                }
            };

            var traefikConfig = new { http = new { routers, services } };
            File.WriteAllText(traefikConfigFile.FullName, Toml.WriteString(traefikConfig));
        }

        public void StopProxy(string name)
        {
            FileInfo traefikConfigFile = new(Path.Combine(_configDirectory, $"{name}.toml"));
            traefikConfigFile.Delete();
        }

        private Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tcs = new TaskCompletionSource();
            _logger.LogInformation($"Traefik is starting");

            try
            {
                Directory.CreateDirectory(_configDirectory);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _binPath,
                        Arguments = "--entrypoints.web.address=" + _listenAddress + " --providers.file.directory " + _configDirectory + " --providers.file.watch=true"
                    }
                };
                var stopTokenRegistration = stoppingToken.Register(() =>
                {
                    process.Kill();
                    process.Dispose();
                });
                process.Exited += (sender, arguments) =>
                {
                    stopTokenRegistration.Dispose();
                    if (process.ExitCode != 0)
                    {
                        string errorMessage = process.StandardError.ReadToEnd();
                        tcs.SetException(new InvalidOperationException("Traefik did not exit correctly. Error message: " + errorMessage));
                    }
                    process.Dispose();
                };
                process.Start();
                _logger.LogInformation($"Traefik is now listening on: http://" + _listenAddress);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                tcs.SetException(e);
            }
            return tcs.Task;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // if the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // otherwise it's running
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                                                            cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Dispose();
        }
    }
}
