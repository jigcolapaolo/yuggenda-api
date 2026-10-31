namespace Yuggenda.Application.DTOs.Customers;

public class CustomerResponse
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}