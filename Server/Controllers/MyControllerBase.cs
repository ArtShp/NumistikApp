using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using System.Security.Claims;

namespace Server.Controllers;

public class MyControllerBase : ControllerBase
{
    protected const int DefaultPageSize = 10;

    protected Guid? GetAuthorizedUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;

        return userId;
    }

    protected UserAppRole? GetAuthorizedUserRole()
    {
        var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        if (userRoleClaim == null || !Enum.TryParse<UserAppRole>(userRoleClaim.Value, out var userRole))
            return null;

        return userRole;
    }
}
