using Hippo.Infrastructure.Data.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Data;

public class SqliteDbContext : ApplicationDbContext
{
    public IConfiguration Configuration { get; }

    public SqliteDbContext(
        IConfiguration configuration,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(mediator, auditableEntitySaveChangesInterceptor)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(Configuration.GetConnectionString("Database"));

        base.OnConfiguring(options);
    }
}
