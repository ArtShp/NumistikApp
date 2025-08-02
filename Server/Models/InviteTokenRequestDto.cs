using Server.Entities;

namespace Server.Models;

public static class InviteTokenDto
{
    public class Request
    {
        public required UserAppRole AssignedRole { get; set; }
    }

    public class Response
    {
        public required Guid Token { get; set; }

        public required UserAppRole AssignedRole { get; set; }

        public required DateTime ExpirationDate { get; set; }
    }
}
