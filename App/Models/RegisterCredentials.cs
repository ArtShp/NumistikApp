namespace App.Models;

public class RegisterCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string InviteToken { get; set; }

    public RegisterCredentials()
    {
        Username = string.Empty;
        Password = string.Empty;
        InviteToken = string.Empty;
    }
}
