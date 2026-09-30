using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(service => service.Id);

        builder.Property(service => service.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(service => service.Description)
            .HasMaxLength(1000);

        builder.Property(service => service.DurationMinutes)
            .IsRequired();

        builder.Property(service => service.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(service => service.IsActive)
            .IsRequired();

        builder.Property(service => service.CreatedAt)
            .IsRequired();

        builder.Property(service => service.UpdatedAt);

        builder.HasOne(service => service.Business)
            .WithMany(business => business.Services)
            .HasForeignKey(service => service.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(service => service.Members)
            .WithMany(member => member.Services)
            .UsingEntity<Dictionary<string, object>>(
                "Service_Member",
                right => right
                    .HasOne<BusinessMember>()
                    .WithMany()
                    .HasForeignKey("BusinessMemberId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<Service>()
                    .WithMany()
                    .HasForeignKey("ServiceId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("ServiceId", "BusinessMemberId");
                });
    }
}