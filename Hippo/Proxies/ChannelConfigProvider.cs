using System;
using System.Collections.Generic;
using System.Linq;
using Hippo.Config;
using Hippo.Models;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Proxies
{
    public class ChannelConfigProvider : IProxyConfigProvider, IReverseProxy
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

        public void StopProxy(Channel channel)
        {
            _logger.LogTrace($"Processing Proxy Stop Request for Application: {channel.Application.Id} Channel: {channel.Id} Host: {channel.Domain.Name}");
            var key = GetKey(channel);
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

            if (removedRoute || removedCluster)
            {
                UpdateConfig();
            }

        }

        public void StartProxy(Channel channel, string address)
        {
            _logger.LogTrace($"Processing Proxy Start Request for Application: {channel.Application.Id} Channel: {channel.Id} Host: {channel.Domain.Name} Address: {address}");
            var key = GetKey(channel);
            var clusterConfig = new ClusterConfig()
            {
                ClusterId = key,
                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                        {
                            {
                                key, new DestinationConfig()
                                {
                                    Address = address
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
                            channel.Domain.Name
                        }
                }
            };

            _routes[key] = routeConfig;
            UpdateConfig();
        }

        private static string GetKey(Channel c) => $"{c.Id}-{c.Application.Id}";
    }
}
