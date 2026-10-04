using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.DTOs.Authentication;
using Yuggenda.Application.Services.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserRegistrationService _userRegistrationService;
    private readonly UserLoginService _userLoginService;

    public AuthController(
        UserRegistrationService userRegistrationService,
        UserLoginService userLoginService
    )
    {
        _userRegistrationService = userRegistrationService;
        _userLoginService = userLoginService;
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

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _userLoginService.LoginAsync(
            request,
            cancellationToken
        );

        return Ok(response);
    }
}