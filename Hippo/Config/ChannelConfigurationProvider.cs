using System;
using System.Collections.Generic;
using System.Globalization;
using Hippo.Models;
using Microsoft.Extensions.Configuration;

namespace Hippo.Config
{
    public class ChannelConfigurationProvider : ConfigurationProvider, IChannelConfigurationProvider
    {
        const string ConfigPrefix = "Wagi:Bindles";
        const string DefaultEndpointKey = "Kestrel:Endpoints:Http:Url";
        const string DefaultEndpointValue = "http://127.0.0.1:0";
        private readonly Dictionary<Guid, ChannelDetails> _channelDetailsDictionary = new();

        public ChannelConfigurationProvider()
        {
            // Kestrel needs at least one endpoint to listen on this is removed as soon as the first channel is added.
            Data.Add(DefaultEndpointKey, DefaultEndpointValue);
        }

        public void SetBindleServer(string bindleServer)
            => Data.AddOrUpdate("Wagi:BindleServer", bindleServer);

        public void AddChannel(Channel channel, string listenAddress, ICollection<EnvironmentVariable> env)
        {
            _channelDetailsDictionary.AddOrUpdate(channel.Id, new ChannelDetails(channel, listenAddress, env));

            if (!Uri.TryCreate(listenAddress, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException($"Listen Address for Channel Id {channel.Id} Name: {channel.Name} ListenAddress: {listenAddress}.");
            }

            var listenAddressKey = $"Kestrel:Endpoints:{channel.Id}:Url";
            Data.AddOrUpdate(listenAddressKey, listenAddress);
            var bindleConfigPrefix = $"{ConfigPrefix}:{channel.Id}";
            var bindleKey = $"{bindleConfigPrefix}:Name";
            var bindleValue = $"{channel.Application.StorageId}/{channel.ActiveRevision.RevisionNumber}";
            Data.AddOrUpdate(bindleKey, bindleValue);
            var hostNamesKey = $"{bindleConfigPrefix}:Hostnames:";
            var host = $"{uri.Host}:{uri.Port}";
            Data.AddOrUpdate(hostNamesKey, host);
            var routeKey = $"{bindleConfigPrefix}:Route";
            Data.AddOrUpdate(routeKey, "/");

            foreach (var envVar in env)
            {
                var envKey = $"{bindleConfigPrefix}:Environment:{envVar.Key}";
                Data.AddOrUpdate(envKey, envVar.Value);
            }

            Data.TryDelete(DefaultEndpointKey);
            OnReload();
        }

        public override void Load()
        {
            OnReload();
        }

        public void RemoveChannel(Channel channel)
        {
            var channelDetails = _channelDetailsDictionary[channel.Id];
            var listenAddress = channelDetails.listenAddress;
            var env = channelDetails.env;
            var listenAddressKey = $"Kestrel:Endpoints:{channel.Id}:Url";
            Data.TryDelete(listenAddressKey);
            var listenPrefix = $"{ConfigPrefix}:{listenAddress}";
            var bindleKey = $"{listenPrefix}:Name";
            Data.TryDelete(bindleKey);

            foreach (var envVar in env)
            {
                var envKey = $"{listenPrefix}:Environment:{envVar.Key}";
                Data.TryDelete(envKey);
            }

            _channelDetailsDictionary.Remove(channel.Id);
            OnReload();
        }

    }

    public static class IDictionaryExtension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryAdd(key, value))
            {
                dictionary[key] = value;
            }
        }

        public static bool TryDelete<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary.Remove(key);
            }
            return false;
        }
    }

    record ChannelDetails(Channel channel, string listenAddress, ICollection<EnvironmentVariable> env);
}
