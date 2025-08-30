using App.Models;

namespace App.Services;

internal class LoginService(IRestApiService service) : ILoginService
{
    private readonly IRestApiService _restApiService = service;

    public Task<bool> TryLoginAsync(LoginCredentials creds)
    {
        return Task.FromResult(true);
    }
}
