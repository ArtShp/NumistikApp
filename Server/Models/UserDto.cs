using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class UserDto
{
    [Required]
    [RegularExpression(@"^\w{3,30}$", ErrorMessage = "Username must be 3-30 characters and contain only letters, numbers, or underscores.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be minimum 8 characters, at least 1 letter and 1 number.")]
    public string Password { get; set; } = string.Empty;
}
