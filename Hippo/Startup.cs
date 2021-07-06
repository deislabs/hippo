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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Hippo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<Account, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<DataContext>();

            // cookie auth for web forms, and token auth for the API.
            services.AddAuthentication().AddCookie().AddJwtBearer(
              cfg =>
              cfg.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = Configuration["Jwt:Issuer"],
                  ValidAudience = Configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
              }
            );

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                    policy => policy.RequireRole("Administrator"));
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            if (HostingEnvironment.IsDevelopment())
            {
                if (Configuration["InMemoryDB"] == "true")
                {
                    services.AddDbContext<DataContext, InMemoryDataContext>();
                }
                else
                {
                    services.AddDbContext<DataContext, SqliteDataContext>();
                }
            }
            else
            {
                services.AddDbContext<DataContext, PostgresDataContext>();
            }

            services.AddScoped<ICurrentUser, ActionContextCurrentUser>();
            services.AddScoped<IUnitOfWork, DbUnitOfWork>();

            services.AddTransient<DataSeeder>();

            var schedulerVar = Environment.GetEnvironmentVariable("HIPPO_JOB_SCHEDULER");
            switch (schedulerVar)
            {
                case "systemd":
                    services.AddSingleton<IJobScheduler, SystemdJobScheduler>();
                    break;
                default:
                    services.AddSingleton<IJobScheduler, WagiLocalJobScheduler>();
                    break;
            }

            services.AddSingleton<ITaskQueue<ChannelReference>, TaskQueue<ChannelReference>>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "hippo API", Version = "v1" });
                var filePath = Path.Combine(AppContext.BaseDirectory, "Hippo.xml");
                if (File.Exists(filePath))
                {
                    c.IncludeXmlComments(filePath);
                }
                c.AddSecurityDefinition("http", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "http" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            services.AddRouting(options => options.LowercaseUrls = true);
            services
                .AddMvc()
                .AddJsonOptions(
                    options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
                );
            services
                .AddReverseProxy()
                .LoadFromHippoChannels();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "hippo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            if (HostingEnvironment.IsDevelopment() && Configuration["InMemoryDB"] != "true")
            {
                using var scope = app.ApplicationServices.CreateScope();
                var dataContext = scope.ServiceProvider.GetService<DataContext>();
                dataContext.Database.Migrate();
            }

            CreateRoles(serviceProvider);

            if (HostingEnvironment.IsDevelopment())
            {
                using var scope = app.ApplicationServices.CreateScope();
                var seeder = scope.ServiceProvider.GetService<DataSeeder>();
                seeder.Seed().Wait();
            }

            {
                using var scope = app.ApplicationServices.CreateScope();
                var scheduler = scope.ServiceProvider.GetService<IJobScheduler>();
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var allApplications = unitOfWork.Applications.ListApplicationsForAllUsers();
                scheduler.OnSchedulerStart(allApplications);
            }
        }

        private static void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            Task<IdentityResult> roleResult;

            Task<bool> hasAdminRole = roleManager.RoleExistsAsync("Administrator");
            hasAdminRole.Wait();

            if (!hasAdminRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Administrator"));
                roleResult.Wait();
            }
        }

    }
}
