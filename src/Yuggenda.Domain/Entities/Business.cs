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
        name = name.Trim();
        timezone = timezone.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Business name is required.",
                nameof(name)
            );
        }

        if (string.IsNullOrWhiteSpace(timezone))
        {
            throw new ArgumentException(
                "Business timezone is required.",
                nameof(timezone)
            );
        }

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            throw new ArgumentException(
                "Business timezone is invalid.",
                nameof(timezone)
            );
        }
        catch (InvalidTimeZoneException)
        {
            throw new ArgumentException(
                "Business timezone is invalid.",
                nameof(timezone)
            );
        }

        Id = Guid.NewGuid();
        Name = name;
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        Email = string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();
        Phone = string.IsNullOrWhiteSpace(phone)
            ? null
            : phone.Trim();
        Timezone = timezone;
        CreatedAt = DateTime.UtcNow;
    }

    
    public void Update(
        string name,
        string? description,
        string? email,
        string? phone,
        string timezone)
    {
        name = name.Trim();
        timezone = timezone.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Business name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(timezone))
        {
            throw new ArgumentException("Business timezone is required.", nameof(timezone));
        }

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            throw new ArgumentException("Business timezone is invalid.", nameof(timezone));
        }
        catch (InvalidTimeZoneException)
        {
            throw new ArgumentException("Business timezone is invalid.", nameof(timezone));
        }

        Name = name;

        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        Email = string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();

        Phone = string.IsNullOrWhiteSpace(phone)
            ? null
            : phone.Trim();

        Timezone = timezone;
        UpdatedAt = DateTime.UtcNow;
    }
}
