using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.Abstractions.Authentication;
using Yuggenda.Application.DTOs.Authentication;
using Yuggenda.Application.DTOs.Users;
using Yuggenda.Application.Services.Authentication;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserRegistrationService _userRegistrationService;
    private readonly UserLoginService _userLoginService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly ICurrentUser _currentUser;

    public AuthController(
        UserRegistrationService userRegistrationService,
        UserLoginService userLoginService,
        RefreshTokenService refreshTokenService,
        ICurrentUser currentUser
    )
    {
        _userRegistrationService = userRegistrationService;
        _userLoginService = userLoginService;
        _refreshTokenService = refreshTokenService;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(
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

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _refreshTokenService.RefreshAsync(request, cancellationToken);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        await _refreshTokenService.LogoutAsync(_currentUser.UserId, request, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        await _refreshTokenService.LogoutAllAsync(_currentUser.UserId, cancellationToken);

        return NoContent();
    }
}