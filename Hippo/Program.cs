
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Hippo.Config;
using Hippo.Proxies;
using Hippo.Tasks;
using Hippo.WagiDotnet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: System.CLSCompliant(false)]

namespace Hippo
{
    public static class Program
    {
        // TODO get value from configuration.
        public static string JobScheduler => Environment.GetEnvironmentVariable("HIPPO_JOB_SCHEDULER")?.ToUpperInvariant() ?? default;
        private static string proxyPort = string.Empty;
        public static string ProxyPort => Program.proxyPort;

        public static void Main(string[] args)
        {
            var tasks = new List<Task>();
            IHostBuilder hippoHostBuilder;

            if (JobScheduler == "WAGI-DOTNET")
            {
                var channelConfigProvider = new ChannelConfigurationProvider();
                var wagiDotnetBuilder = CreateWagiDotnetHostBuilder(channelConfigProvider);
                var wagiDotnetHost = wagiDotnetBuilder.Build();
                tasks.Add(wagiDotnetHost.RunAsync());
                hippoHostBuilder = CreateHippoHostBuilder(args, channelConfigProvider);
            }
            else
            {
                hippoHostBuilder = CreateHostBuilder(args);
            }

            var hippoHost = hippoHostBuilder.Build();
            var proxyUpdateTaskQueue = hippoHost.Services.GetRequiredService<ITaskQueue<ReverseProxyUpdateRequest>>();
            var proxyHostBuilder = CreateProxyHostBuilder(proxyUpdateTaskQueue);
            var proxyHost = proxyHostBuilder.Build();
            proxyPort = GetProxyHTTPSPort(proxyHost.Services.GetRequiredService<IConfiguration>());
            tasks.Add(hippoHost.RunAsync());
            tasks.Add(proxyHost.RunAsync());
            Task.WaitAny(tasks.ToArray());
        }

        /// <summary>
        /// Gets the HTTPS Proxy port so that links can be created correctly in the Hippo UI. this will need to be updated for external schedulers.
        /// </summary>
        /// <param name="config">The Proxy Configuration</param>
        /// <returns>The proy HTTPS Port as a string</returns>

        static string GetProxyHTTPSPort(IConfiguration config)
        {
            var port = string.Empty;
            var proxyUrl = config?.GetValue<string>("Kestrel:Endpoints:Https:Url");
            if (!string.IsNullOrEmpty(proxyUrl) && Uri.TryCreate(proxyUrl, UriKind.Absolute, out Uri result))
            {
                port = result.Port == 0 || result.Port == 443 ? string.Empty : $":{result.Port.ToString(CultureInfo.InvariantCulture)}";
            }
            return port;
        }

        // This has to be called CreateHostBuilder because the ef migrations tool looks specifically
        // for that method name.
        // NOTE do not run the ef migrations tool with env var HIPPO_JOB_SCHEDULER set to WAGI-DOTNET as it will fail.

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return CreateHippoHostBuilder(args, null);
        }

        static IHostBuilder CreateHippoHostBuilder(string[] args, ChannelConfigurationProvider channelConfigurationProvider)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                                  .UseConsoleLifetime()
                                  .ConfigureWebHostDefaults(webBuilder =>
                                  {
                                      webBuilder.UseStartup<Startup>();
                                  })
                                  .ConfigureServices(services =>
                                  {
                                      services.AddHostedService<ChannelUpdateBackgroundService>();
                                  });

            if (channelConfigurationProvider is not null)
            {
                hostBuilder.ConfigureServices(services =>
                {
                    services.AddSingleton<IChannelConfigurationProvider>(channelConfigurationProvider);
                });
            }

            return hostBuilder;
        }

        static IHostBuilder CreateProxyHostBuilder(ITaskQueue<ReverseProxyUpdateRequest> proxyUpdateTaskQueue) =>
            Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), "Proxies"))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<ProxyStartup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables("HIPPO_REVERSE_PROXY_");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(proxyUpdateTaskQueue);
                    services.AddHostedService<ReverseProxyUpdateBackgroundService>();
                });

        static IHostBuilder CreateWagiDotnetHostBuilder(ChannelConfigurationProvider channelConfigurationProvider) =>
            Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), "WagiDotnet"))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<WagiDotnetStartup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddChannelConfiguration(channelConfigurationProvider)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables("WAGI_DOTNET_");

                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IChannelConfigurationProvider>(channelConfigurationProvider);
                });
    }
}
