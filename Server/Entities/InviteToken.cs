using Microsoft.EntityFrameworkCore;

namespace Server.Entities;

[Index(nameof(Token), IsUnique = true)]
public class InviteToken
{
    public int Id { get; set; }

    public Guid Token { get; set; }

    public required User CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public Role AssignedRole { get; set; }

    public User? UsedBy { get; set; }

    public DateTime? UsedAt { get; set; }
}
