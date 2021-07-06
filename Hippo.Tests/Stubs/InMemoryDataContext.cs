using Hippo.Models;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Tests.Stubs
{
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
    }
}
