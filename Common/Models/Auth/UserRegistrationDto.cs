using Shared.Models.Extensions;
using Shared.Models.Common;

namespace Shared.Models.Auth;

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
