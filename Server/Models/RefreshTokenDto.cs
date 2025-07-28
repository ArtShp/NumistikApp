using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public static class RefreshTokenDto
{
    public class Request
    {
        [RegularExpression(@"^\w{3,30}$", ErrorMessage = "Username must be 3-30 characters and contain only letters, numbers, or underscores.")]
        public required string Username { get; set; }

        public required string RefreshToken { get; set; }
    }

    public class Response
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
