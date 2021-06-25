using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Models
{
    public class DataContext : IdentityDbContext<Account>
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<EnvironmentVariable> EnvironmentVariables { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Revision> Revisions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: there must be a cleaner way using the abstract BaseEntity class here...
            // meh. do what works for now.
            modelBuilder.Entity<Application>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Application>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Channel>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Channel>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Configuration>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Configuration>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Domain>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Domain>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<EnvironmentVariable>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<EnvironmentVariable>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Key>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Key>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Revision>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Revision>().Property(x => x.Modified).HasDefaultValueSql("now()");

            modelBuilder.Entity<Application>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Application>()
                .HasMany(a => a.Channels)
                .WithOne(c => c.Application);

            modelBuilder.Entity<Application>()
                .HasMany(a => a.Revisions)
                .WithOne(r => r.Application);

            modelBuilder.Entity<Configuration>()
                .HasMany(c => c.EnvironmentVariables)
                .WithOne(e => e.Configuration);

            modelBuilder.Entity<Domain>()
                .HasIndex(d => d.Name)
                .IsUnique();

            modelBuilder.Entity<Channel>()
                .Property(c => c.RevisionSelectionStrategy)
                .HasConversion<int>();
        }
    }
}
