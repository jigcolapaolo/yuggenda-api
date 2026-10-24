using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Services;

public class UpdateServiceRequest
{
    [StringLength(150, MinimumLength = 2)]
    public string? Name { get; init; }

    [StringLength(1000)]
    public string? Description { get; init; }

    [Range(1, 1440)]
    public int? DurationMinutes { get; init; }

    [Range(typeof(decimal), "0,01", "9999999999999999,99")]
    public decimal? Price { get; init; }
}