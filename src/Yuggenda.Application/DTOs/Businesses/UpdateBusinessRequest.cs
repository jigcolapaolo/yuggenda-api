using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Businesses;

public class UpdateBusinessRequest
{
    [MaxLength(150)]
    public string? Name { get; init; }

    [MaxLength(500)]
    public string? Description { get; init; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; init; }

    [MaxLength(50)]
    public string? Phone { get; init; }

    [MaxLength(100)]
    public string? Timezone { get; init; }
}