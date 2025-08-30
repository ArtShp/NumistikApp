namespace App.Models;

public class LoginCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }

    public LoginCredentials()
    {
        Username = string.Empty;
        Password = string.Empty;
    }
}
