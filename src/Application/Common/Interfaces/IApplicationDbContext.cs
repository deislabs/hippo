using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<App> Apps { get; }

    DbSet<Certificate> Certificates { get; }

    DbSet<Channel> Channels { get; }

    DbSet<EnvironmentVariable> EnvironmentVariables { get; }

    DbSet<Revision> Revisions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
