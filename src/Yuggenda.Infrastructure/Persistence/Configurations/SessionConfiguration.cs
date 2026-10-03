using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yuggenda.Domain.Entities;

namespace Yuggenda.Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(session => session.Id);

        builder.Property(session => session.RefreshTokenHash)
            .IsRequired();

        builder.Property(session => session.ExpiresAt)
            .IsRequired();

        builder.Property(session => session.CreatedAt)
            .IsRequired();

        builder.Property(session => session.RevokedAt);

        builder.HasOne(session => session.User)
            .WithMany(user => user.Sessions)
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(session => session.UserId);

        builder.HasIndex(session => session.ExpiresAt);
    }
}