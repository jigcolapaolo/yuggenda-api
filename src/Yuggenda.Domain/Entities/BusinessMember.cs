using Yuggenda.Domain.Enums;

namespace Yuggenda.Domain.Entities;

public class BusinessMember
{
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public Guid UserId { get; private set; }
    public BusinessRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public BusinessMember(
        Guid businessId,
        Guid userId,
        BusinessRole role
    )
    {
        Id = Guid.NewGuid();
        BusinessId = businessId;
        UserId = userId;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}