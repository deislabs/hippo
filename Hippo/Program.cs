
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Hippo.Proxies;
using Hippo.Tasks;

[assembly: System.CLSCompliant(false)]

namespace Hippo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var hippoHostBuilder = CreateHippoHostBuilder(args);
            var hippoHost = hippoHostBuilder.Build();
            var proxyUpdateTaskQueue = hippoHost.Services.GetRequiredService<ITaskQueue<ReverseProxyUpdateRequest>>();
            var proxyHostBuilder = CreateProxyHostBuilder(proxyUpdateTaskQueue);
            var proxyHost = proxyHostBuilder.Build();
            var hippoTask = hippoHost.RunAsync();
            var proxyTask = proxyHost.RunAsync();
            Task.WaitAll(new Task[] { hippoTask, proxyTask });
        }

        static IHostBuilder CreateHippoHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ChannelUpdateBackgroundService>();
                });

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
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables("HIPPO_REVERSE_PROXY_");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(proxyUpdateTaskQueue);
                    services.AddHostedService<ReverseProxyUpdateBackgroundService>();
                });

    }
}
