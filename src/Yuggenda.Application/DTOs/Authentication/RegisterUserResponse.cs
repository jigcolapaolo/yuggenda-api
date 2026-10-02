namespace Yuggenda.Application.DTOs.Authentication;

public class RegisterUserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}