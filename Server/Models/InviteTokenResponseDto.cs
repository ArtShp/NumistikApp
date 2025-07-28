using Server.Entities;

namespace Server.Models;

public class InviteTokenResponseDto
{
    public required Guid Token { get; set; }
    public required Role AssignedRole { get; set; }
    public required DateTime ExpirationDate { get; set; }
}
