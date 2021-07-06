using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hippo.Models
{
    public class DataContext : IdentityDbContext<Account>
    {
        private protected readonly IConfiguration _configuration;

        public DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<EnvironmentVariable> EnvironmentVariables { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Revision> Revisions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // TODO: there must be a cleaner way using the abstract BaseEntity class here...
            // meh. do what works for now.
            builder.Entity<Application>().Property(x => x.Created).HasDefaultValueSql("now()");
            builder.Entity<Application>().Property(x => x.Modified).HasDefaultValueSql("now()");
            builder.Entity<Channel>().Property(x => x.Created).HasDefaultValueSql("now()");
            builder.Entity<Channel>().Property(x => x.Modified).HasDefaultValueSql("now()");
            builder.Entity<Configuration>().Property(x => x.Created).HasDefaultValueSql("now()");
            builder.Entity<Configuration>().Property(x => x.Modified).HasDefaultValueSql("now()");
            builder.Entity<Domain>().Property(x => x.Created).HasDefaultValueSql("now()");
            builder.Entity<Domain>().Property(x => x.Modified).HasDefaultValueSql("now()");
            builder.Entity<EnvironmentVariable>().Property(x => x.Created).HasDefaultValueSql("now()");
            builder.Entity<EnvironmentVariable>().Property(x => x.Modified).HasDefaultValueSql("now()");
            builder.Entity<Key>().Property(x => x.Created).HasDefaultValueSql("now()");
            builder.Entity<Key>().Property(x => x.Modified).HasDefaultValueSql("now()");
            builder.Entity<Revision>().Property(x => x.Created).HasDefaultValueSql("now()");
            builder.Entity<Revision>().Property(x => x.Modified).HasDefaultValueSql("now()");

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
    }
}
