using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Yuggenda.Application.Abstractions.Authentication;

namespace Yuggenda.Api.Authentication;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new UnauthorizedAccessException("Invalid user identity.");
            }

            return parsedUserId;
        }
    }
}