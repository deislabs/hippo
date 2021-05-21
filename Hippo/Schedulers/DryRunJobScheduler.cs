using System.Linq;
using Hippo.Models;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public class DryRunJobScheduler: IJobScheduler
    {
        private readonly ILogger _logger;

        public DryRunJobScheduler(ILogger<DryRunJobScheduler> logger)
        {
            _logger = logger;   
        }
        public void Start(Channel c)
        {
            _logger.LogInformation($"Started channel: for app {c.Application.Name} channel {c.Name}");
            _logger.LogInformation($"- Channel: id: {c.Id}");
            _logger.LogInformation($"- Release: revision: {c.Release.Revision}, artifacts: {c.Release.UploadUrl}");
            _logger.LogInformation($"- Network: domain: '{c.Domain?.Name ?? "(null)"}' port: {c.PortID + Channel.EphemeralPortRange}");
            LogEnvironment(c);
        }

        public void Stop(Channel c)
        {
            _logger.LogInformation($"Stopped channel: app {c.Application.Name} channel {c.Name}");
        }

        private void LogEnvironment(Channel c)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                foreach (var ev in c.Configuration.EnvironmentVariables)
                {
                    _logger.LogTrace($"- Environment: {ev.Key}={ev.Value}");
                }
            }
            else
            {
                _logger.LogInformation($"- Environment: {c.Configuration.EnvironmentVariables.Count()} variables defined (Trace level to dump)");
            }
        }
    }
}