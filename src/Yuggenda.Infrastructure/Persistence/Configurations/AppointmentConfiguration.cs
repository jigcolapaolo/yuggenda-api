using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(appointment => appointment.Id);

        builder.Property(appointment => appointment.StartTime)
            .IsRequired();

        builder.Property(appointment => appointment.EndTime)
            .IsRequired();

        builder.Property(appointment => appointment.Status)
            .IsRequired();

        builder.Property(appointment => appointment.Notes)
            .HasMaxLength(1000);

        builder.Property(appointment => appointment.CreatedAt)
            .IsRequired();

        builder.Property(appointment => appointment.UpdatedAt);

        builder.HasOne(appointment => appointment.Business)
            .WithMany(business => business.Appointments)
            .HasForeignKey(appointment => appointment.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(appointment => appointment.Customer)
            .WithMany(customer => customer.Appointments)
            .HasForeignKey(appointment => appointment.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(appointment => appointment.Service)
            .WithMany(service => service.Appointments)
            .HasForeignKey(appointment => appointment.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(appointment => appointment.BusinessMember)
            .WithMany(member => member.Appointments)
            .HasForeignKey(appointment => appointment.BusinessMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(appointment => new
        {
            appointment.BusinessId,
            appointment.StartTime
        });

        builder.HasIndex(appointment => new
        {
            appointment.BusinessMemberId,
            appointment.StartTime
        });

        builder.HasIndex(appointment => new
        {
            appointment.CustomerId,
            appointment.StartTime
        });
    }
}
