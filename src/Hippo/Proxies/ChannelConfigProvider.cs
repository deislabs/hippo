using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hippo.Config;
using Hippo.Models;
using Hippo.Tasks;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Proxies
{
    public class ChannelConfigProvider : IProxyConfigProvider, IReverseProxyUpdater
    {
        private volatile ChannelConfig _config;
        private readonly ILogger<ChannelConfigProvider> _logger;
        private readonly Dictionary<string, RouteConfig> _routes;
        private readonly Dictionary<string, ClusterConfig> _clusters;

        public ChannelConfigProvider(ILogger<ChannelConfigProvider> logger)
        {
            _routes = new Dictionary<string, RouteConfig>();
            _clusters = new Dictionary<string, ClusterConfig>();
            _config = new ChannelConfig();
            _logger = logger;
        }

        public IProxyConfig GetConfig() => _config;

        public void UpdateConfig()
        {
            _logger.LogTrace($"Updating YARP Config");
            var existingConfig = _config;
            var routes = _routes.Values.ToList<RouteConfig>();
            var clusters = _clusters.Values.ToList<ClusterConfig>();
            var newConfig = new ChannelConfig(routes, clusters);
            _config = newConfig;

            // Need to signal to YARP to get the new configuration by calling SignalChange on the existingConfiguration.
            existingConfig.SignalChange();
        }

        public bool UpdateProxyRecord(ReverseProxyUpdateRequest record)
        {
            if (record.Action == ReverseProxyAction.Stop)
            {
                return DeleteProxyRecord(record);
            }

            if (record.Action == ReverseProxyAction.Start)
            {
                AddOrUpdateProxyRecord(record);
                return true;
            }

            return false;
        }
        private void AddOrUpdateProxyRecord(ReverseProxyUpdateRequest record)
        {
            _logger.LogTrace($"Processing Proxy Start Request for Application: {record.ApplicationId} Channel: {record.ChannelId} Host: {record.Domain} Address: {record.Host}");
            var key = GetKey(record.ApplicationId, record.ChannelId);
            var clusterConfig = new ClusterConfig()
            {
                ClusterId = key,
                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                        {
                            {
                                key, new DestinationConfig()
                                {
                                    Address = record.Host
                                }
                            }
                        }
            };

            _clusters[key] = clusterConfig;

            var routeConfig = new RouteConfig()
            {
                RouteId = key,
                ClusterId = key,
                Match = new RouteMatch
                {
                    Hosts = new List<string>()
                        {
                            record.Domain
                        }
                }
            };

            _routes[key] = routeConfig;
        }

        private bool DeleteProxyRecord(ReverseProxyUpdateRequest record)
        {
            _logger.LogTrace($"Processing Proxy Stop Request for Application: {record.ApplicationId} Channel: {record.ChannelId} Host: {record.Domain}");
            var key = GetKey(record.ApplicationId, record.ChannelId);
            var removedRoute = _routes.Remove(key);
            if (!removedRoute)
            {
                _logger.LogError($"Attempted to remove route for key:{key} but it did not exist in Dictionary");
            }

            var removedCluster = _clusters.Remove(key);
            if (!removedCluster)
            {
                _logger.LogError($"Attempted to remove cluster for key:{key} but it did not exist in Dictionary");
            }

            return removedCluster || removedRoute;
        }

        private static string GetKey(Guid appId, Guid channelId) => $"App:{appId}-Channel:{channelId}";
    }
}
