using Server.Models.Extensions;

namespace Server.Models.Auth;

public static class UserLoginDto
{
    public class Request
    {
        [Username]
        public required string Username { get; set; }

        [Password]
        public required string Password { get; set; }
    }
}
