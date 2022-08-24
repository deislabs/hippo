using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Hippo.Application;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using Hippo.Application.Jobs;
using Hippo.Core.Entities;
using Hippo.Infrastructure;
using Hippo.Infrastructure.Data;
using Hippo.Infrastructure.Identity;
using Hippo.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Respawn;

namespace Hippo.FunctionalTests;

public class TestBase : IDisposable
{
    private static IConfigurationRoot _configuration = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Checkpoint _checkpoint = null!;
    private static string? _currentUserId;
    private static Random random = new Random();

    public TestBase()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        var services = new ServiceCollection();

        services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
            w.EnvironmentName == "Development" &&
            w.ApplicationName == "Hippo.Web"));

        services.AddLogging();

        services.AddApplication();
        services.AddInfrastructure(_configuration);

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddSingleton<IConfiguration>(_configuration);

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
                    .AddDbContextCheck<ApplicationDbContext>();

        services.AddControllersWithViews().AddFluentValidation();

        services.AddRouting(options => options.LowercaseUrls = true);

        services.Configure<ApiBehaviorOptions>(options =>
                    options.SuppressModelStateInvalidFilter = true);

        // Register testing services
        services.AddTransient(provider =>
            Mock.Of<ICurrentUserService>(s => s.UserId == _currentUserId));

        services.AddTransient<IStorageService, FakeStorageService>();

        services.AddSingleton<IJobService, NullNomadService>();

        _scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

        _checkpoint = new Checkpoint
        {
            TablesToIgnore = new[] { "__EFMigrationsHistory" }
        };

        EnsureDatabase();
    }

    private static void EnsureDatabase()
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (_configuration.GetValue<string>("Database:Driver", "inmemory") != "inmemory")
        {
            context.Database.Migrate();
        }
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { UserRole.Administrator });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();

        var user = new Account { UserName = userName, Email = userName };

        var result = await userManager.CreateAsync(user, password);

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRolesAsync(user, roles);
        }

        if (result.Succeeded)
        {
            _currentUserId = user.Id;

            return _currentUserId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    public async void Dispose()
    {
        await _checkpoint.Reset("Database");

        _currentUserId = null;
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static IConfiguration GetConfiguration()
    {
        return _configuration;
    }
}

public class NullNomadService : IJobService
{
    public void StartJob(Guid id, string bindleId, Dictionary<string, string> environmentVariables, string? domain)
    {
        new NullJob();
    }

    public void DeleteJob(string jobId)
    {

    }

    public string[] GetJobLogs(string jobName)
    {
        return new string[] { };
    }

    public IEnumerable<Job>? GetJobs()
    {
        return null;
    }

    public Job? GetJob(string jobName)
    {
        return null;
    }

    private class NullJob : Job
    {
        public void Reload() { }

        public void Run()
        {
        }

        public void Stop()
        {
        }
    }
}
