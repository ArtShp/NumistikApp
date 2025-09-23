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

    public Task<bool> TryReLoginAsync()
    {
        return _restApiService.ReAuthorize(new RefreshTokenDto.Request
        {
            Username = AppSettings.Username,
            RefreshToken = AppSettings.RefreshToken
        });
    }

    public async Task<bool> Register(UserRegistrationDto.Request requestBody)
    {
        UserRegistrationDto.Response? result = await _restApiService.SendRestApiRequest(RestApiEndpoints.Register, requestBody);

        if (result != null)
        {
            return true;
        }

        return false;
    }

    public async Task<InviteTokenDto.Response?> CreateInviteToken(InviteTokenDto.Request requestBody)
    {
        return await _restApiService.SendRestApiRequest(RestApiEndpoints.CreateInviteToken, requestBody);
    }
}
