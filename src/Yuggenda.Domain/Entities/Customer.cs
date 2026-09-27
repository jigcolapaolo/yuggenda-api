namespace Yuggenda.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public Guid? UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Customer(
        Guid businessId,
        string firstName,
        string lastName,
        string? email = null,
        string? phone = null,
        Guid? userId = null
    )
    {
        Id = Guid.NewGuid();
        BusinessId = businessId;
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        CreatedAt = DateTime.UtcNow;
    }
}