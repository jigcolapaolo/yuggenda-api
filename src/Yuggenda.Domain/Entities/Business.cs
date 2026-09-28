namespace Yuggenda.Domain.Entities;

public class Business
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string Timezone { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public ICollection<BusinessMember> Members { get; private set; }
        = new List<BusinessMember>();

    public ICollection<Service> Services { get; private set; }
        = new List<Service>();

    public ICollection<Customer> Customers { get; private set; }
        = new List<Customer>();

    public ICollection<Appointment> Appointments { get; private set; }
        = new List<Appointment>();

    public Business(
        string name,
        string timezone,
        string? description = null,
        string? email = null,
        string? phone = null
    )
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Email = email;
        Phone = phone;
        Timezone = timezone;
        CreatedAt = DateTime.UtcNow;
    }
}