using App.Models;
using Shared.Models.Auth;

namespace App.Services;

internal class LoginService(IRestApiService service) : ILoginService
{
    private readonly IRestApiService _restApiService = service;

    public Task<bool> TryLoginAsync(LoginCredentials creds)
    {
        return _restApiService.Authorize(new UserLoginDto.Request
        {
            Username = creds.Username,
            Password = creds.Password
        });
    }
}
