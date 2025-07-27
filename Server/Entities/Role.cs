using Microsoft.AspNetCore.Authorization;

namespace Server.Entities;

public enum Role
{
    User = 1,
    Admin = 2,
    Owner = 3
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class AuthorizeRoleAttribute : AuthorizeAttribute
{
    public AuthorizeRoleAttribute(params Role[] roles)
    {
        Roles = string.Join(",", roles.Select(r => Role.GetName(r)));
    }
}
