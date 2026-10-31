using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Customers;

public class UpdateCustomerRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }
}