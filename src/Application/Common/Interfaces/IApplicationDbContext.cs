using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Hippo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }

    DbSet<App> Apps { get; }

    DbSet<Certificate> Certificates { get; }

    DbSet<Channel> Channels { get; }

    DbSet<EnvironmentVariable> EnvironmentVariables { get; }

    DbSet<Revision> Revisions { get; }

    ChangeTracker ChangeTracker { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
