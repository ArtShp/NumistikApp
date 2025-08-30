using App.Models;

namespace App.Services;

public interface ILoginService
{
    Task<bool> TryLoginAsync(LoginCredentials creds);
}