using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [RegularExpression(@"^\w{3,30}$", ErrorMessage = "Username must be 3-30 characters and contain only letters, numbers, or underscores.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = Role.User;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
}
