using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

[Index(nameof(Token), IsUnique = true)]
public class InviteToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Token { get; set; }

    [Required]
    public required User CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    public Role AssignedRole { get; set; }

    public User? UsedBy { get; set; }

    public DateTime? UsedAt { get; set; }
}
