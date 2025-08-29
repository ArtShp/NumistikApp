using Microsoft.EntityFrameworkCore;
using Server.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

[Index(nameof(Token), IsUnique = true)]
public class InviteToken
{
    public int Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Token { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public Guid CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public UserAppRole AssignedRole { get; set; }

    [ForeignKey(nameof(UsedBy))]
    public Guid? UsedById { get; set; }

    public User? UsedBy { get; set; }

    public DateTime? UsedAt { get; set; }
}
