using Yuggenda.Domain.Enums;

namespace Yuggenda.Domain.Entities;

public class BusinessMember
{
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public Guid UserId { get; private set; }
    public BusinessRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = null!;
    public Business Business { get; private set; } = null!;
    public ICollection<Availability> Availabilities { get; private set; }
        = new List<Availability>();
    public ICollection<Service> Services { get; private set; }
        = new List<Service>();
    public ICollection<Appointment> Appointments { get; private set; }
        = new List<Appointment>();

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