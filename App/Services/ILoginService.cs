using App.Models;

namespace App.Services;

internal interface ILoginService
{
    Task<bool> TryLoginAsync(LoginCredentials creds);
}