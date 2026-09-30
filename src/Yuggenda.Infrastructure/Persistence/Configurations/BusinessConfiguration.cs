using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.HasKey(business => business.Id);

        builder.Property(business => business.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(business => business.Description)
            .HasMaxLength(1000);

        builder.Property(business => business.Email)
            .HasMaxLength(255);

        builder.Property(business => business.Phone)
            .HasMaxLength(50);

        builder.Property(business => business.Timezone)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(business => business.CreatedAt)
            .IsRequired();

        builder.Property(business => business.UpdatedAt);
    }
}