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
            Data[DefaultEndpointKey] = DefaultEndpointValue;
        }

        public void SetBindleServer(string bindleServer)
            => Data["Wagi:BindleServer"] = bindleServer;

        public void AddChannel(Channel channel, string listenAddress)
        {
            _channelDetailsDictionary[channel.Id] = new ChannelDetails(channel, listenAddress);

            if (!Uri.TryCreate(listenAddress, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException($"Listen Address for Channel Id {channel.Id} Name: {channel.Name} ListenAddress: {listenAddress}.");
            }

            var listenAddressKey = $"Kestrel:Endpoints:{channel.Id}:Url";
            Data[listenAddressKey] = listenAddress;
            var bindleConfigPrefix = $"{ConfigPrefix}:{channel.Id}";
            var bindleKey = $"{bindleConfigPrefix}:Name";
            var bindleValue = $"{channel.Application.StorageId}/{channel.ActiveRevision.RevisionNumber}";
            Data[bindleKey] = bindleValue;
            var hostNamesKey = $"{bindleConfigPrefix}:Hostnames:";
            var host = $"{uri.Host}:{uri.Port}";
            Data[hostNamesKey] = host;
            var routeKey = $"{bindleConfigPrefix}:Route";
            Data[routeKey] = "/";

            foreach (var envVar in channel.GetEnvironmentVariables())
            {
                var envKey = $"{bindleConfigPrefix}:Environment:{envVar.Key}";
                Data[envKey] = envVar.Value;
            }

            Data.Remove(DefaultEndpointKey);
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
            var listenAddressKey = $"Kestrel:Endpoints:{channel.Id}:Url";
            Data.Remove(listenAddressKey);
            var listenPrefix = $"{ConfigPrefix}:{listenAddress}";
            var bindleKey = $"{listenPrefix}:Name";
            Data.Remove(bindleKey);

            foreach (var envVar in channel.GetEnvironmentVariables())
            {
                var envKey = $"{listenPrefix}:Environment:{envVar.Key}";
                Data.Remove(envKey);
            }

            _channelDetailsDictionary.Remove(channel.Id);
            OnReload();
        }

    }
    record ChannelDetails(Channel channel, string listenAddress);
}
