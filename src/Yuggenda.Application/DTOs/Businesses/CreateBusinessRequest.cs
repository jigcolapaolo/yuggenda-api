using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Businesses;

public class CreateBusinessRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; init; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; init; }

    [MaxLength(50)]
    public string? Phone { get; init; }

    [Required]
    [MaxLength(100)]
    public string Timezone { get; init; } = string.Empty;
}