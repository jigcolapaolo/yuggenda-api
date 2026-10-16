namespace Yuggenda.Application.DTOs.Services;

public class ServiceResponse
{
    public Guid Id { get; init; }
    public Guid BusinessId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DurationMinutes { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; } 
}