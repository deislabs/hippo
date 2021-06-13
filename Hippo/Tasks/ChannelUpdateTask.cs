using System;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Schedulers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Tasks
{
    public class ChannelUpdateTask : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ITaskQueue<ChannelReference> _queue;
        private readonly ILogger _logger;

        public ChannelUpdateTask(IServiceProvider services, ITaskQueue<ChannelReference> queue, ILogger<ChannelUpdateTask> logger)
        {
            _services = services;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var channelReference = await _queue.Dequeue(stoppingToken);
                _logger.LogTrace($"ExecuteAsync: dequeued app {channelReference.ApplicationId}, channel {channelReference.ChannelId}");
                using (var scope = _services.CreateScope())
                {
                    using (var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>())
                    {
                        try
                        {
                            var channel = unitOfWork.Channels.GetChannelById(channelReference.ChannelId);
                            // TODO: should we make this responsible for updating the active revision
                            var scheduler = scope.ServiceProvider.GetRequiredService<IJobScheduler>();
                            // TODO: do any schedulers need the channel info *before* ActiveRevision
                            // got updated in order to stop?
                            try
                            {
                                _logger.LogInformation($"ExecuteAsync: redeploying {channel.Application.Name} channel {channel.Name} at rev {channel.ActiveRevision}");
                                scheduler.Stop(channel);
                                scheduler.Start(channel);
                                _logger.LogTrace($"ExecuteAsync: redeployed {channel.Application.Name} channel {channel.Name} at rev {channel.ActiveRevision}");
                            }
                            catch (Exception e)
                            {
                                // Catch here to provide more informative error message
                                _logger.LogError($"ExecuteAsync: failed to redeploy {channel.Application.Name} channel {channel.Name} at rev {channel.ActiveRevision}: {e}");
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"ChannelUpdateTask: error processing {channelReference.ChannelId}: {e}");
                        }
                        // TODO: should failed channels be retried or put on a manual queue or something?
                    }
                }
            }
        }
    }

    public record ChannelReference(Guid ApplicationId, Guid ChannelId);
}
