using FluentValidation.AspNetCore;
using Hippo.Application;
using Hippo.Application.Common.Config;
using Hippo.Application.Common.Interfaces;
using Hippo.Infrastructure;
using Hippo.Infrastructure.Data;
using Hippo.Infrastructure.HealthChecks;
using Hippo.Infrastructure.Identity;
using Hippo.Web.Extensions;
using Hippo.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>()
            .AddCheck<BindleHealthCheck>("Bindle")
            .AddCheck<NomadHealthCheck>("Nomad");

builder.Services.AddControllers()
    .AddNewtonsoftJson()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddFluentValidation();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

builder.Services.AddAuthentication().AddCookie().AddJwtBearer(cfg =>
    {
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        cfg.SaveToken = true;
    }
);

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(0, 1);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("Api-Version");
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseFileServer();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();


app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        "api_route",
        "API",
        "api/{controller}/{action}/{id?}"
    );
    endpoints.MapCustomHealthChecks("/healthz");
    endpoints.MapSwagger();
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        var databaseDriver = builder.Configuration.GetValue<string>("Database:Driver");
        if (databaseDriver != "inmemory")
        {
            context.Database.Migrate();
        }

        var userManager = services.GetRequiredService<UserManager<Account>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await ApplicationDbContextSeed.SeedIdentityRolesAsync(roleManager);

        HippoConfig hippoConfig = new HippoConfig();
        builder.Configuration.GetSection("Hippo").Bind(hippoConfig);

        foreach (var admin in hippoConfig.Administrators)
        {
            await ApplicationDbContextSeed.SeedAdministratorAccountsAsync(userManager, admin.Username, admin.Username, admin.Password);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        throw;
    }
}

app.Run();
