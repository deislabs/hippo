using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hippo.Infrastructure.Data.Configurations;

public class EnvironmentVariableConfiguration : IEntityTypeConfiguration<EnvironmentVariable>
{
    public void Configure(EntityTypeBuilder<EnvironmentVariable> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(e => e.Value)
            .IsRequired();
    }
}
