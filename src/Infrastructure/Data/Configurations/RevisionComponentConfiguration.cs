using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hippo.Infrastructure.Data.Configurations;

public class RevisionComponentConfiguration : IEntityTypeConfiguration<RevisionComponent>
{
    public void Configure(EntityTypeBuilder<RevisionComponent> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(256);
    }
}
