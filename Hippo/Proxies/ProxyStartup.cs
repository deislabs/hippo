using System;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Hippo.Extensions;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Schedulers;
using Hippo.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


namespace Hippo.Proxies
{
    public class ProxyStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReverseProxy()
                .LoadFromHippoChannels();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // TODO: Configure an exception handler.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
            });
        }
    }
}
