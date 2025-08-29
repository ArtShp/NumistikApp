using Shared.Models.Extensions;

namespace Shared.Models.Auth;

public static class RefreshTokenDto
{
    public class Request
    {
        [Username]
        public required string Username { get; set; }

        public required string RefreshToken { get; set; }
    }

    public class Response
    {
        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }
    }
}
