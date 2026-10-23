using Yuggenda.Domain.Enums;

namespace Yuggenda.Application.DTOs.Services;

public class ServiceMemberResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public BusinessRole Role { get; init; }
}
