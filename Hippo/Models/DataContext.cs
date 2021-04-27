using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Models
{
    public class DataContext: IdentityDbContext<Account>
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<App> Applications { get; set; }
        public DbSet<Build> Builds { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Config> Configuration { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<EnvironmentVariable> EnvironmentVariables { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Release> Releases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: there must be a cleaner way using the abstract BaseEntity class here...
            // meh. do what works for now.
            modelBuilder.Entity<App>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<App>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Build>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Build>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Certificate>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Certificate>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Config>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Config>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Domain>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Domain>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<EnvironmentVariable>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<EnvironmentVariable>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Key>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Key>().Property(x => x.Modified).HasDefaultValueSql("now()");
            modelBuilder.Entity<Release>().Property(x => x.Created).HasDefaultValueSql("now()");
            modelBuilder.Entity<Release>().Property(x => x.Modified).HasDefaultValueSql("now()");

            modelBuilder.Entity<App>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Domain>()
                .HasIndex(d => d.Name)
                .IsUnique();
        }
    }
}
