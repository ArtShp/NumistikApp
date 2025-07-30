using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class UsernameAttribute : RegularExpressionAttribute
{
    public UsernameAttribute()
        : base(@"^\w{3,30}$")
    {
        ErrorMessage = "Username must be 3-30 characters and contain only letters, numbers, or underscores.";
    }
}

public class PasswordAttribute : RegularExpressionAttribute
{
    public PasswordAttribute()
        : base(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")
    {
        ErrorMessage = "Password must be minimum 8 characters, at least 1 letter and 1 number.";
    }
}
