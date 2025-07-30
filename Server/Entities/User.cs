using Microsoft.EntityFrameworkCore;

namespace Server.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.User;

    public string? RefreshTokenHash { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
}
