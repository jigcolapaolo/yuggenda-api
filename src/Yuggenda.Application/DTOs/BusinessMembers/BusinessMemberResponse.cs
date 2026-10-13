using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.DTOs.BusinessMembers;

public class BusinessMemberResponse
{
    public Guid Id { get; init; }
    public Guid BusinessId { get; init; }
    public Guid UserId { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}