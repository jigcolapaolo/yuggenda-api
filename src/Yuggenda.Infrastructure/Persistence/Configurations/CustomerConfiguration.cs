using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(customer => customer.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(customer => customer.Email)
            .HasMaxLength(255);

        builder.Property(customer => customer.Phone)
            .HasMaxLength(50);

        builder.Property(customer => customer.CreatedAt)
            .IsRequired();

        builder.Property(customer => customer.UpdatedAt);

        builder.HasOne(customer => customer.Business)
            .WithMany(business => business.Customers)
            .HasForeignKey(customer => customer.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(customer => customer.User)
            .WithMany(user => user.Customers)
            .HasForeignKey(customer => customer.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}