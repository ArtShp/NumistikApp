using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : MyControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserRegistrationDto.Response>> Register(UserRegistrationDto.Request request)
    {
        var userRole = await authService.RegisterAsync(request);

        if (userRole is null)
            return BadRequest("Some error while registration of the user.");

        return StatusCode(201, userRole);
    }

    [HttpPost("login")]
    public async Task<ActionResult<RefreshTokenDto.Response>> Login(UserLoginDto.Request request)
    {
        var result = await authService.LoginAsync(request);

        if (result is null)
            return Unauthorized("Invalid username or password.");

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenDto.Response>> RefreshToken(RefreshTokenDto.Request request)
    {
        var result = await authService.RefreshTokensAsync(request);

        if (result is null || result.AccessToken is null || result.RefreshToken is null)
            return Unauthorized("Invalid refresh token or username.");

        return Ok(result);
    }

    [HttpPost("create-invite-token")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<InviteTokenDto.Response>> CreateInviteToken(InviteTokenDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        // Get the user's role from claims
        UserAppRole? userRole = GetAuthorizedUserRole();
        if (userRole == null)
            return StatusCode(403, "User role is missing or invalid.");

        if (request.AssignedRole >= userRole)
            return StatusCode(403, $"You don't have enough rights to create invite tokens for role {request.AssignedRole}.");

        var result = await authService.CreateInviteTokenAsync(authenticatedUserId.Value, request.AssignedRole);

        if (result is null)
            return BadRequest("Failed to create invite token.");

        return Ok(result);
    }
}
