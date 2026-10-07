using System.ComponentModel.DataAnnotations;

namespace Yuggenda.Application.DTOs.Authentication;

public class LogoutRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}