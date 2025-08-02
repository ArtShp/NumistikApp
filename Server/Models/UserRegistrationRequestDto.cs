using Server.Entities;

namespace Server.Models;

public static class UserRegistrationDto
{
    public class Request
    {
        [Username]
        public required string Username { get; set; }

        [Password]
        public required string Password { get; set; }

        public Guid? InviteToken { get; set; } = null;
    }

    public class Response
    {
        public required UserAppRole Role { get; set; }
    }
}
