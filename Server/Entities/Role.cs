using Microsoft.AspNetCore.Authorization;

namespace Server.Entities;

public enum Role
{
    User = 1,
    Admin = 2,
    Owner = 3
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public abstract class AuthorizeRoleAttribute : AuthorizeAttribute
{
    public AuthorizeRoleAttribute(params Role[] roles)
    {
        Roles = string.Join(",", roles.Select(r => Role.GetName(r)));
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class AuthorizeAllUsersAttribute : AuthorizeRoleAttribute
{
    public AuthorizeAllUsersAttribute() : base([Role.User, Role.Admin, Role.Owner]) { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class AuthorizeOnlyAdminsAttribute : AuthorizeRoleAttribute
{
    public AuthorizeOnlyAdminsAttribute() : base([Role.Admin, Role.Owner]) { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class AuthorizeOnlyOwnerAttribute : AuthorizeRoleAttribute
{
    public AuthorizeOnlyOwnerAttribute() : base([Role.Owner]) { }
}
