using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Configurations;

public class BusinessMemberConfiguration : IEntityTypeConfiguration<BusinessMember>
{
    public void Configure(EntityTypeBuilder<BusinessMember> builder)
    {
        builder.HasKey(member => member.Id);

        builder.Property(member => member.Role)
            .IsRequired();

        builder.Property(member => member.CreatedAt)
            .IsRequired();

        builder.HasOne(member => member.User)
            .WithMany(user => user.BusinessMemberships)
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(member => member.Business)
            .WithMany(business => business.Members)
            .HasForeignKey(member => member.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(member => new
        {
            member.BusinessId,
            member.UserId
        })
        .IsUnique();
    }
}
