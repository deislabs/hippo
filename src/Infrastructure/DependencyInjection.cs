using Hippo.Application.Common.Configuration;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Identity;
using Hippo.Infrastructure.Data;
using Hippo.Infrastructure.Exceptions;
using Hippo.Infrastructure.Files;
using Hippo.Infrastructure.Identity;
using Hippo.Infrastructure.Jobs;
using Hippo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hippo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // NOTE(bacongobbler): to create a new migration, run `dotnet ef migrations add "SampleMigration" --project src/Infrastructure --startup-project src/Web --output-dir Data/Migrations`
        var driver = configuration.GetValue<string>("Database:Driver", "inmemory").ToLower();
        switch (driver)
        {
            case "inmemory":
                services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("Hippo"));
                break;
            case "postgresql":
                services.AddDbContext<ApplicationDbContext, PostgresqlDbContext>();
                break;
            case "sqlite":
                services.AddDbContext<ApplicationDbContext, SqliteDbContext>();
                break;
            default:
                throw new InvalidDatabaseDriverException(driver);
        }

        HippoConfiguration hippoConfiguration = new HippoConfiguration();
        configuration.GetSection("Hippo").Bind(hippoConfiguration);
        services.AddSingleton<HippoConfiguration>(hippoConfiguration);

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddIdentity<Account, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddTransient<ISignInService, SignInService>();

        services.AddTransient<IAccessTokenService, AccessTokenService>();

        services.AddTransient<IRefreshTokenService, RefreshTokenService>();

        var schedulerDriver = configuration.GetValue<string>("Scheduler:Driver", "local").ToLower();
        switch (schedulerDriver)
        {
            case "local":
                services.AddSingleton<IJobFactory, LocalJobFactory>();
                break;
            case "nomad":
                services.AddSingleton<IJobFactory, NomadJobFactory>();
                break;
        }

        services.AddTransient<IJsonFileBuilder, JsonFileBuilder>();

        services.AddAuthorization(options =>
                options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}
