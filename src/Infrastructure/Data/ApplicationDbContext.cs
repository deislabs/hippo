using System.Reflection;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Common;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using Hippo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<Account>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    private readonly IDateTime _dateTime;

    public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IDateTime dateTime) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public ApplicationDbContext(
            ICurrentUserService currentUserService,
            IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public DbSet<App> Apps => Set<App>();

    public DbSet<Certificate> Certificates => Set<Certificate>();

    public DbSet<Channel> Channels => Set<Channel>();

    public DbSet<EnvironmentVariable> EnvironmentVariables => Set<EnvironmentVariable>();

    public DbSet<Revision> Revisions => Set<Revision>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.UtcNow;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<IHasDomainEvent>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    var createdEvent = (DomainEvent)Activator.CreateInstance(typeof(CreatedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity)!;
                    entry.Entity.DomainEvents.Add(createdEvent);
                    break;
                case EntityState.Modified:
                    var modifiedEvent = (DomainEvent)Activator.CreateInstance(typeof(ModifiedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity)!;
                    entry.Entity.DomainEvents.Add(modifiedEvent);
                    break;
                case EntityState.Deleted:
                    var deletedEvent = (DomainEvent)Activator.CreateInstance(typeof(DeletedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity)!;
                    entry.Entity.DomainEvents.Add(deletedEvent);
                    break;
            }
        }

        var events = ChangeTracker.Entries<IHasDomainEvent>()
            .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
            .Where(domainEvent => !domainEvent.IsPublished)
            .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
