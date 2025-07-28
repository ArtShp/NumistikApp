using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models;
using Server.Services;
using System.Security.Claims;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserRegistrationDto>> Register(UserRegistrationDto request)
    {
        var userRole = await authService.RegisterAsync(request);

        if (userRole is null)
            return BadRequest("Some error while registration of the user.");

        return StatusCode(201, userRole);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
    {
        var result = await authService.LoginAsync(request);

        if (result is null)
            return Unauthorized("Invalid username or password.");

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await authService.RefreshTokensAsync(request);

        if (result is null || result.AccessToken is null || result.RefreshToken is null)
            return Unauthorized("Invalid refresh token or user id.");

        return Ok(result);
    }

    [HttpPost("create-invite-token")]
    [AuthorizeRole(Role.Admin, Role.Owner)]
    public async Task<ActionResult<InviteToken>> CreateInviteToken(InviteTokenDto request)
    {
        // Get the user's id from claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User is not authenticated.");

        if (!Guid.TryParse(userIdClaim.Value, out var authenticatedUserId))
            return Unauthorized("Invalid user id.");

        // Get the user's role from claims
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (roleClaim == null || !Enum.TryParse<Role>(roleClaim.Value, out var userRole))
            return StatusCode(403, "User role is missing or invalid.");

        if (request.AssignedRole >= userRole)
            return StatusCode(403, $"You don't have enough rights to create invite tokens for role {request.AssignedRole}.");

        var result = await authService.CreateInviteTokenAsync(authenticatedUserId, request.AssignedRole);

        if (result is null)
            return BadRequest("Failed to create invite token.");

        return Ok(result);
    }
}
