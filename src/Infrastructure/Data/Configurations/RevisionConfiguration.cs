using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hippo.Infrastructure.Data.Configurations;

public class RevisionConfiguration : IEntityTypeConfiguration<Revision>
{
    public void Configure(EntityTypeBuilder<Revision> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.Property(r => r.RevisionNumber)
            .IsRequired()
            .HasMaxLength(256);
    }
}
