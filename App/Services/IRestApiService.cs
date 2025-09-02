using Shared.Models.Auth;

namespace App.Services;

public interface IRestApiService
{
    Task<bool> Authorize(UserLoginDto.Request requestBody);
    
    Task<bool> ReAuthorize(RefreshTokenDto.Request requestBody);

    void Logout();

    Task<TResponse?> SendRestApiRequest<TResponse>(
        RestApiEndpoint<TResponse> endpoint
    );

    Task<TResponse?> SendRestApiRequest<TRequest, TResponse>(
        RestApiEndpoint<TRequest, TResponse> endpoint, TRequest? requestBody = null
    ) where TRequest : class;
}

public abstract class RestApiEndpoint(HttpMethod httpMethod, string endpoint, bool requiresAuth)
{
    public HttpMethod HttpMethod { get; init; } = httpMethod;
    public string Endpoint { get; init; } = endpoint;
    public bool RequiresAuth { get; init; } = requiresAuth;
}

public class RestApiEndpoint<TResponse>(HttpMethod httpMethod, string endpoint, bool requiresAuth) : 
    RestApiEndpoint(httpMethod, endpoint, requiresAuth) {}

public class RestApiEndpoint<TRequest, TResponse>(HttpMethod httpMethod, string endpoint, bool requiresAuth) : 
    RestApiEndpoint(httpMethod, endpoint, requiresAuth) where TRequest : class {}

internal partial class RestApiEndpoints
{
    
}
