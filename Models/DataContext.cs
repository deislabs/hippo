using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippo.Models
{
    public class DataContext: IdentityDbContext<Account>
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<App> Applications { get; set; }
        public DbSet<Config> Configuration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>()
                .HasIndex(a => a.Name)
                .IsUnique();
            modelBuilder.Entity<App>()
                .Property(b => b.Created)
                .HasDefaultValueSql("now()");
            modelBuilder.Entity<App>()
                .Property(b => b.Modified)
                .HasDefaultValueSql("now()");

            base.OnModelCreating(modelBuilder);
        }
    }
}
