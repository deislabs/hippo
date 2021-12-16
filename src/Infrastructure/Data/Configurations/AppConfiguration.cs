using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hippo.Infrastructure.Data.Configurations;

public class AppConfiguration : IEntityTypeConfiguration<App>
{
    public void Configure(EntityTypeBuilder<App> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.StorageId)
            .IsRequired()
            .HasMaxLength(200);
    }
}
