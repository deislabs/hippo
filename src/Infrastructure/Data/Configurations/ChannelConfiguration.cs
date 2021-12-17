using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hippo.Infrastructure.Data.Configurations;

public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(c => c.Domain)
            .IsRequired();
    }
}
