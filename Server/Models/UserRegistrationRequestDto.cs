using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public static class UserRegistrationDto
{
    public class Request
    {
        [RegularExpression(@"^\w{3,30}$", ErrorMessage = "Username must be 3-30 characters and contain only letters, numbers, or underscores.")]
        public required string Username { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be minimum 8 characters, at least 1 letter and 1 number.")]
        public required string Password { get; set; } = string.Empty;

        public Guid? InviteToken { get; set; } = null;
    }

    public class Response
    {
        public required Role Role { get; set; }
    }
}
