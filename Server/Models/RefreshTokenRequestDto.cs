namespace Server.Models;

public class RefreshTokenRequestDto
{
    public required string Username { get; set; }
    public required string RefreshToken { get; set; }
}
