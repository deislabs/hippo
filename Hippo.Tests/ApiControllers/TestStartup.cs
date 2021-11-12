using System.Reflection;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tasks;
using Hippo.Tests.Stubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Hippo.Tests.ApiControllers
{
    public class TestStartup
    {
        private readonly MockTokenIssuer _tokenIssuer;
        private readonly ITaskQueue<ChannelReference> _taskQueue;
        private readonly string _testDatabaseName;

        public TestStartup(MockTokenIssuer tokenIssuer, string testDatabaseName, ITaskQueue<ChannelReference> taskQueue)
        {
            Assert.NotNull(tokenIssuer);
            _tokenIssuer = tokenIssuer;
            Assert.False(string.IsNullOrEmpty(testDatabaseName));
            _testDatabaseName = testDatabaseName;
            _taskQueue = taskQueue;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<Account, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<DataContext>();
            services.AddControllers().AddApplicationPart(Assembly.Load("hippo-server")).AddControllersAsServices();

            services.AddAuthentication().AddJwtBearer(
                cfg => cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _tokenIssuer.Issuer,
                    ValidAudience = _tokenIssuer.Audience,
                    IssuerSigningKey = _tokenIssuer.SecurityKey
                }
            );

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                    policy => policy.RequireRole("Administrator"));
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddDbContext<DataContext, InMemoryDataContext>(
                options =>
                options.UseInMemoryDatabase(_testDatabaseName)
            );

            services.AddScoped<ICurrentUser, ActionContextCurrentUser>();
            services.AddScoped<IUnitOfWork, DbUnitOfWork>();
            if (_taskQueue != null)
            {
                services.AddSingleton<ITaskQueue<ChannelReference>>(_taskQueue);
            }

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
