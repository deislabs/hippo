using System;
using System.Collections.Generic;
using System.Linq;
using Hippo.Config;
using Hippo.Messages;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Providers
{
    public class ChannelConfigProvider : IProxyConfigProvider, IProxyConfigUpdater
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

        public void UpdateConfig(YarpConfigurationRequest request)
        {
            _logger.LogTrace($"Processing Proxy Config Request for Application:{request.AppId} Channel:{request.ChannelId} Action: {request.Action} Host: {request.Hostname} Backend: {request.Backend}");
            var updated = true;
            var key = $"{request.AppId}-{request.ChannelId}";

            switch (request.Action)
            {
                case YarpConfigurationAction.Start:
                    AddOrUpdateYarpConfiguration(key, request);
                    break;
                case YarpConfigurationAction.Stop:
                    updated = RemoveYarpConfiguration(key);
                    break;
                default:
                    break;
            }

            if (updated)
            {
                _logger.LogTrace($"Updating YARP config for Application:{request.AppId} Channel:{request.ChannelId} Action: {request.Action}");
                var currentConfig = _config;
                var routes = _routes.Values.ToList<RouteConfig>();
                var clusters = _clusters.Values.ToList<ClusterConfig>();
                _config = new ChannelConfig(routes, clusters);
                currentConfig.SignalChange();
            }
        }

        private void AddOrUpdateYarpConfiguration(string key, YarpConfigurationRequest request)
        {
            if (request.Action == YarpConfigurationAction.Start)
            {
                var clusterConfig = new ClusterConfig()
                {
                    ClusterId = key,
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                        {
                            {
                                key, new DestinationConfig()
                                {
                                    Address = $"http://{request.Backend}"
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
                            request.Hostname
                        }
                    }
                };

                _routes[key] = routeConfig;
            }
        }

        private bool RemoveYarpConfiguration(string key)
        {
            var removedRoute = _routes.Remove(key);
            if (!removedRoute)
            {
                _logger.LogError($"Failed to remove route for key:{key}");
            }

            var removedCluster = _clusters.Remove(key);
            if (!removedCluster)
            {
                _logger.LogError($"Failed to remove cluster for key:{key}");
            }

            return removedRoute || removedCluster;
        }
    }
}
