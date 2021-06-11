using System;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.Repositories;
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
            var count = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var channelReference = await _queue.Dequeue(stoppingToken);
                using (var scope = _services.CreateScope())
                {
                    using (var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>())
                    {
                        try
                        {
                            var channel = unitOfWork.Channels.GetChannelById(channelReference.ChannelId);
                            // TODO: should we make this responsible for updating the active revision
                            // TODO: update scheduler
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"ChannelUpdateTask: error processing {channelReference.ChannelId}: {e}");
                        }
                    }
                }
                await Task.Delay(2500, stoppingToken);
                System.Console.WriteLine("BURP" + count++);
            }
        }
    }

    public record ChannelReference(Guid ApplicationId, Guid ChannelId);
}
