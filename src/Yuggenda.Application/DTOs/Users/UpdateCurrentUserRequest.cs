using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Users;

public class UpdateCurrentUserRequest
{
    [EmailAddress]
    public string? Email { get; init; }

    [MaxLength(100)]
    public string? FirstName { get; init; }

    [MaxLength(100)]
    public string? LastName { get; init; }
}