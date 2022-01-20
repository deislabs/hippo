using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hippo.Infrastructure.Data.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(c => c.PublicKey)
            .IsRequired();

        builder.Property(c => c.PrivateKey)
            .IsRequired();
    }
}
