using System.Text;
using System.Text.Json.Serialization;
using Hippo.Config;
using Hippo.Models;
using Hippo.Proxies;
using Hippo.Repositories;
using Hippo.Schedulers;
using Hippo.Tasks;
using Hippo.WagiDotnet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

static WebApplicationBuilder CreateHippoWebApplicationBuilder(string[] args, ChannelConfigurationProvider channelConfigurationProvider)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    builder.Services.AddMvc().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new
                JsonStringEnumConverter()));

    // authentication/authorization
    builder.Services.AddIdentity<Account, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<DataContext>();

    builder.Services.AddAuthentication().AddCookie().AddJwtBearer(cfg =>
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new
            SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            });

    builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                        policy => policy.RequireRole("Administrator"));
            });

    // data context
    var driver = builder.Configuration.GetValue<string>("Database:Driver", "inmemory").ToLower();
    switch (driver)
    {
        case "inmemory":
            builder.Services.AddDbContext<DataContext, InMemoryDataContext>();
            break;
        case "postgresql":
            builder.Services.AddDbContext<DataContext, PostgresDataContext>();
            break;
        case "sqlite":
            builder.Services.AddDbContext<DataContext, SqliteDataContext>();
            break;
        default:
            throw new ArgumentException(String.Format("{0} is not a valid database driver", driver));
    }

    builder.Services.AddScoped<ICurrentUser, ActionContextCurrentUser>();
    builder.Services.AddScoped<IUnitOfWork, DbUnitOfWork>();
    builder.Services.AddSingleton<ITaskQueue<ChannelReference>, TaskQueue<ChannelReference>>();
    builder.Services.AddSingleton<ITaskQueue<ReverseProxyUpdateRequest>, TaskQueue<ReverseProxyUpdateRequest>>();
    builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

    // job scheduler
    builder.Services.AddHostedService<ChannelUpdateBackgroundService>();
    var schedulerDriver = builder.Configuration.GetValue<string>("Scheduler:Driver", "wagi").ToLower();
    switch (schedulerDriver)
    {
        case "wagi-dotnet":
            builder.Services.AddSingleton<JobScheduler, WagiDotnetJobScheduler>();
            break;
        case "wagi":
            builder.Services.AddSingleton<JobScheduler, WagiLocalJobScheduler>();
            break;
        default:
            throw new ArgumentException(String.Format("{0} is not a valid scheduler driver", schedulerDriver));
    }

    builder.WebHost.UseKestrel(options =>
        {
            options.ListenAnyIP(builder.Configuration.GetValue<int>("Kestrel:Endpoints:Http:Port", 5000));
            options.ListenAnyIP(
                builder.Configuration.GetValue<int>("Kestrel:Endpoints:Https:Port", 5001),
                    listenOptions =>
                    {
                        listenOptions.UseHttps();
                    });
        }
    );

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "hippo API",
            Version = "v1"
        });
        c.AddSecurityDefinition("http", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement { {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "http"
                }
            },
            Array.Empty<string>() } });
    });

    return builder;
}

static IHostBuilder CreateProxyHostBuilder(ITaskQueue<ReverseProxyUpdateRequest> proxyUpdateTaskQueue)
{
    var builder = Host.CreateDefaultBuilder()
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
    return builder;
}

static IHostBuilder CreateWagiDotnetHostBuilder(ChannelConfigurationProvider channelConfigurationProvider)
{
    var builder = Host.CreateDefaultBuilder()
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
    return builder;
}

var tasks = new List<Task>();
var builder = CreateHippoWebApplicationBuilder(args, null);

// The WAGI.NET and Hippo hosts share some services
if (builder.Configuration.GetValue<string>("Scheduler:Driver", "wagi").ToLower() == "wagi-dotnet")
{
    var channelConfigProvider = new ChannelConfigurationProvider();
    var wagiDotnetHost = CreateWagiDotnetHostBuilder(channelConfigProvider).Build();
    tasks.Add(wagiDotnetHost.RunAsync());
    builder.Services.AddSingleton<IChannelConfigurationProvider>(channelConfigProvider);
}

var hippoHost = builder.Build();

if (builder.Configuration.GetValue<bool>("ProxyEnabled", false))
{
    var proxyUpdateTaskQueue = hippoHost.Services.GetRequiredService<ITaskQueue<ReverseProxyUpdateRequest>>();
    var proxyHostBuilder = CreateProxyHostBuilder(proxyUpdateTaskQueue);
    var proxyHost = proxyHostBuilder.Build();
    tasks.Add(proxyHost.RunAsync());
}

// Configure the HTTP request pipeline.
if (hippoHost.Environment.IsDevelopment())
{
    hippoHost.UseDeveloperExceptionPage();
    // run database migrations
    using var scope = hippoHost.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetService<DataContext>();
    dataContext.Database.Migrate();
}

hippoHost.UseSwagger();
hippoHost.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "hippo v1"));
hippoHost.UseHttpsRedirection();
hippoHost.UseStaticFiles();
hippoHost.UseRouting();
hippoHost.UseAuthentication();
hippoHost.UseAuthorization();
hippoHost.MapControllerRoute(
    name: "default",
    pattern: "{controller=App}/{action=Index}/{id?}"
);

tasks.Add(hippoHost.RunAsync());

Task.WaitAny(tasks.ToArray());
