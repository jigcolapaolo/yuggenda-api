namespace Yuggenda.Application.DTOs.Businesses;

public class BusinessResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Timezone { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}