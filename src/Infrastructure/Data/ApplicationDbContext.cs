using System.Reflection;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Identity;
using Hippo.Core.Common;
using Hippo.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<Account>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    private readonly IDateTime _dateTime;

    private readonly IDomainEventService _domainEventService;

    public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            IDomainEventService domainEventService) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _domainEventService = domainEventService;
    }

    public ApplicationDbContext(
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            IDomainEventService domainEventService)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _domainEventService = domainEventService;
    }

    public DbSet<App> Apps => Set<App>();

    public DbSet<Certificate> Certificates => Set<Certificate>();

    public DbSet<Channel> Channels => Set<Channel>();

    public DbSet<EnvironmentVariable> EnvironmentVariables => Set<EnvironmentVariable>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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

        var events = ChangeTracker.Entries<IHasDomainEvent>()
            .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
            .Where(domainEvent => !domainEvent.IsPublished)
            .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }
}
