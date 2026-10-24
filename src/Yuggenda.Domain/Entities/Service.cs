using Yuggenda.Domain.Enums;
using Yuggenda.Domain.Exceptions;

namespace Yuggenda.Domain.Entities;

public class Service
{
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Business Business { get; private set; } = null!;
    public ICollection<BusinessMember> Members { get; private set; }
        = new List<BusinessMember>();
    public ICollection<Appointment> Appointments { get; private set; }
        = new List<Appointment>();

    public Service(
        Guid businessId,
        string name,
        int durationMinutes,
        decimal price,
        string? description = null
    )
    {
        Id = Guid.NewGuid();
        BusinessId = businessId;
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        Price = price;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string? description,
        int durationMinutes,
        decimal price
    )
    {
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignMember(BusinessMember member)
    {
        if (member.BusinessId != BusinessId)
        {
            throw new DomainException(
                "The member and service must belong to the same business."
            );
        }

        if (member.Role != BusinessRole.Staff)
        {
            throw new DomainException(
                "Only staff members can be assigned to services."
            );
        }

        if (Members.Any(existing => existing.Id == member.Id))
        {
            throw new DomainException(
                "The member is already assigned to this service."
            );
        }

        Members.Add(member);
    }

    public void RemoveMember(BusinessMember member)
    {
        Members.Remove(member);
    }
}
