using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.DTOs.Authentication;
using Yuggenda.Application.Services.Authentication;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserRegistrationService _userRegistrationService;

    public AuthController(UserRegistrationService userRegistrationService)
    {
        _userRegistrationService = userRegistrationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResponse>> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _userRegistrationService.RegisterAsync(
            request,
            cancellationToken
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }
}