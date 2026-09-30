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
}