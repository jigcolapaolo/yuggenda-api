using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yuggenda.Application.DTOs.Users;
using Yuggenda.Application.Services.Users;

namespace Yuggenda.Api.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe(
        CancellationToken cancellationToken
    )
    {
        var response = await _userService.GetCurrentUserAsync(cancellationToken);

        return Ok(response);
    }

    [HttpPatch("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe(
        UpdateCurrentUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await _userService.UpdateCurrentUserAsync(
            request,
            cancellationToken
        );

        return Ok(response);
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _userService.ChangePasswordAsync(
            request,
            cancellationToken
        );

        return NoContent();
    }
}