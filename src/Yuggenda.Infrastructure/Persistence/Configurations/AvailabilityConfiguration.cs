using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Configurations;

public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.HasKey(availability => availability.Id);

        builder.Property(availability => availability.Type)
            .IsRequired();

        builder.Property(availability => availability.DayOfWeek);

        builder.Property(availability => availability.SpecificDate);

        builder.Property(availability => availability.StartTime)
            .IsRequired();

        builder.Property(availability => availability.EndTime)
            .IsRequired();

        builder.Property(availability => availability.IsAvailable)
            .IsRequired();

        builder.Property(availability => availability.CreatedAt)
            .IsRequired();

        builder.Property(availability => availability.UpdatedAt);

        builder.HasOne(availability => availability.BusinessMember)
            .WithMany(member => member.Availabilities)
            .HasForeignKey(availability => availability.BusinessMemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
