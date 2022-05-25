using Hippo.Application.Common.Config;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
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

        HippoConfig hippoConfig = new HippoConfig();
        configuration.GetSection("Hippo").Bind(hippoConfig);
        services.AddSingleton<HippoConfig>(hippoConfig);

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddIdentity<Account, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddTransient<ISignInService, SignInService>();

        services.AddTransient<ITokenService, TokenService>();

        services.AddTransient<IBindleService, BindleService>();

        services.AddTransient<IJobService, NomadJobService>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequiredUniqueChars = 0;
        });

        services.AddTransient<IJsonFileBuilder, JsonFileBuilder>();

        services.AddAuthorization(options =>
                options.AddPolicy(UserPolicy.CanPurge, policy => policy.RequireRole(UserRole.Administrator)));

        return services;
    }
}
