using Hippo.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Data;

public class SqliteDbContext : ApplicationDbContext
{
    public IConfiguration Configuration { get; }

    public SqliteDbContext(IConfiguration configuration, ICurrentUserService currentUserService, IDateTime dateTime) : base(currentUserService, dateTime)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(Configuration.GetConnectionString("Database"));
    }
}
