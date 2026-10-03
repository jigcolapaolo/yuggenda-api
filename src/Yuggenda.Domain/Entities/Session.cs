namespace Yuggenda.Domain.Entities;

public class Session
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string RefreshTokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public User User { get; private set; } = null!;

    public Session(
        Guid userId,
        string refreshTokenHash,
        DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RefreshTokenHash = refreshTokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }
}