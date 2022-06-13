using Hippo.Infrastructure.Data.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Data;

public class PostgresqlDbContext : ApplicationDbContext
{
    public IConfiguration Configuration { get; }
    public PostgresqlDbContext(
        IConfiguration configuration,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(mediator, auditableEntitySaveChangesInterceptor)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(Configuration.GetConnectionString("Database"));

        base.OnConfiguring(options);
    }
}
