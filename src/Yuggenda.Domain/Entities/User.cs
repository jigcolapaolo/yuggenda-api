namespace Yuggenda.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public ICollection<BusinessMember> BusinessMemberships { get; private set; }
        = new List<BusinessMember>();

    public ICollection<Customer> Customers { get; private set; }
        = new List<Customer>();

    public User(
        string email,
        string passwordHash,
        string firstName,
        string lastName
    )
    {
        Id = Guid.NewGuid();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTime.UtcNow;
    }
}
