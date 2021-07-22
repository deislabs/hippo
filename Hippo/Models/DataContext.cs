using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hippo.Models
{
    public abstract class DataContext : IdentityDbContext<Account>
    {
        private protected readonly IConfiguration _configuration;

        protected DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<EnvironmentVariable> EnvironmentVariables { get; set; }
        public DbSet<EventLogEntry> EventLogEntries { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Revision> Revisions { get; set; }

        private protected abstract string SqlNow { get; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // TODO: there must be a cleaner way using the abstract BaseEntity class here...
            // meh. do what works for now.
            builder.Entity<Application>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<Application>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);
            builder.Entity<Channel>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<Channel>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);
            builder.Entity<Configuration>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<Configuration>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);
            builder.Entity<Domain>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<Domain>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);
            builder.Entity<EnvironmentVariable>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<EnvironmentVariable>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);
            builder.Entity<EventLogEntry>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<EventLogEntry>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);
            builder.Entity<Key>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<Key>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);
            builder.Entity<Revision>().Property(x => x.Created).HasDefaultValueSql(SqlNow);
            builder.Entity<Revision>().Property(x => x.Modified).HasDefaultValueSql(SqlNow);

            builder.Entity<Application>()
                .HasIndex(a => a.Name)
                .IsUnique();

            builder.Entity<Application>()
                .HasMany(a => a.Channels)
                .WithOne(c => c.Application);

            builder.Entity<Application>()
                .HasMany(a => a.Revisions)
                .WithOne(r => r.Application);

            builder.Entity<Configuration>()
                .HasMany(c => c.EnvironmentVariables)
                .WithOne(e => e.Configuration);

            builder.Entity<Domain>()
                .HasIndex(d => d.Name)
                .IsUnique();

            builder.Entity<Channel>()
                .Property(c => c.RevisionSelectionStrategy)
                .HasConversion<int>();

            builder.Entity<EventLogEntry>()
                .Property(c => c.EventKind)
                .HasConversion<int>();

            builder.Entity<EventLogEntry>()
                .Property(c => c.EventSource)
                .HasConversion<int>();
        }
    }

    public class PostgresDataContext : DataContext
    {
        public PostgresDataContext(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Hippo"));
        }

        private protected override string SqlNow => "now()";
    }

    public class SqliteDataContext : DataContext
    {
        public SqliteDataContext(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("Hippo"));
        }

        private protected override string SqlNow => "datetime('now')";
    }

    // Convenient for dev-test when the feature requires exploratory changes to
    // the database schema
    public class InMemoryDataContext : DataContext
    {
        private readonly string _databaseName;

        public InMemoryDataContext(string databaseName = "Hippo") : base(null)
        {
            _databaseName = databaseName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase(_databaseName);
            }
        }

        private protected override string SqlNow => "now()";
    }
}
