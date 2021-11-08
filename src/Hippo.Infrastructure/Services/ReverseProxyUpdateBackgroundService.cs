using System;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Core.Interfaces;
using Hippo.Core.ReverseProxies;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Infrastructure.Services
{
    public class ReverseProxyUpdateBackgroundService : BackgroundService
    {
        private readonly IReverseProxyUpdater _reverseProxyUpdater;
        private readonly ITaskQueue<ReverseProxyUpdateRequest> _queue;
        private readonly ILogger _logger;

        public ReverseProxyUpdateBackgroundService(IReverseProxyUpdater reverseProxyUpdater, ITaskQueue<ReverseProxyUpdateRequest> queue, ILogger<ReverseProxyUpdateBackgroundService> logger)
        {
            _reverseProxyUpdater = reverseProxyUpdater;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var configUpdated = false;
                var moreRecords = false;
                var reverseProxyUpdateRequest = await _queue.Dequeue(stoppingToken);
                do
                {
                    try
                    {
                        _logger.LogTrace($"ExecuteAsync: Dequeued Proxy Update Request AppId: {reverseProxyUpdateRequest.ApplicationId}, ChannelId: {reverseProxyUpdateRequest.ChannelId} Domain: {reverseProxyUpdateRequest.Domain} Action: {reverseProxyUpdateRequest.Action}");
                        var updated = _reverseProxyUpdater.UpdateProxyRecord(reverseProxyUpdateRequest);
                        configUpdated = configUpdated || updated;
                        (moreRecords, reverseProxyUpdateRequest) = _queue.TryRead();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"ReverseProxyUpdateTask: error processing  AppId: {reverseProxyUpdateRequest.ApplicationId}, ChannelId: {reverseProxyUpdateRequest.ChannelId} Domain: {reverseProxyUpdateRequest.Domain}: {e}");
                    }
                } while (moreRecords);

                if (configUpdated)
                {
                    _reverseProxyUpdater.UpdateConfig();
                }

            }

            _logger.LogTrace("ExecuteAsync: ReverseProxyUpdateRequest - Cancellation Requested");
        }
    }
}
