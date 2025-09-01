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

public abstract class RestApiEndpoint(HttpMethod httpMethod, string endpoint)
{
    public HttpMethod HttpMethod { get; init; } = httpMethod;
    public string Endpoint { get; init; } = endpoint;
}

public class RestApiEndpoint<TResponse>(HttpMethod httpMethod, string endpoint) : 
    RestApiEndpoint(httpMethod, endpoint) {}

public class RestApiEndpoint<TRequest, TResponse>(HttpMethod httpMethod, string endpoint) : 
    RestApiEndpoint(httpMethod, endpoint) where TRequest : class {}

internal partial class RestApiEndpoints
{
    
}
