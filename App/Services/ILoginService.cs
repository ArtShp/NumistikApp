using App.Models;
using Shared.Models.Auth;

namespace App.Services;

public interface ILoginService
{
    Task<bool> TryLoginAsync(LoginCredentials creds);

    Task<bool> TryReLoginAsync();

    Task<bool> Register(UserRegistrationDto.Request requestBody);
}

internal partial class RestApiEndpoints
{
    public static readonly RestApiEndpoint<UserLoginDto.Request, RefreshTokenDto.Response>
        Login = new(HttpMethod.Post, "Auth/login", false);

    public static readonly RestApiEndpoint<RefreshTokenDto.Request, RefreshTokenDto.Response>
        ReLogin = new(HttpMethod.Post, "Auth/refresh-token", false);

    public static readonly RestApiEndpoint<UserRegistrationDto.Request, UserRegistrationDto.Response>
        Register = new(HttpMethod.Post, "Auth/register", false);
}
