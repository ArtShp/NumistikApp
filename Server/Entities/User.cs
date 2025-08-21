using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Server.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserAppRole Role { get; set; }

    public string? RefreshTokenHash { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<UserCollection> UserCollections { get; set; } = [];
}



public enum UserAppRole
{
    User = 1,
    Admin = 2,
    Owner = 3
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public abstract class AuthorizeRoleAttribute : AuthorizeAttribute
{
    public AuthorizeRoleAttribute(params UserAppRole[] roles)
    {
        Roles = string.Join(",", roles.Select(r => UserAppRole.GetName(r)));
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class AuthorizeAllUsersAttribute : AuthorizeRoleAttribute
{
    public AuthorizeAllUsersAttribute() : base([UserAppRole.User, UserAppRole.Admin, UserAppRole.Owner]) { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class AuthorizeOnlyAdminsAttribute : AuthorizeRoleAttribute
{
    public AuthorizeOnlyAdminsAttribute() : base([UserAppRole.Admin, UserAppRole.Owner]) { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class AuthorizeOnlyOwnerAttribute : AuthorizeRoleAttribute
{
    public AuthorizeOnlyOwnerAttribute() : base([UserAppRole.Owner]) { }
}
