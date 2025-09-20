using Shared.Models.Auth;

namespace App.Services;

public interface IRestApiService
{
    Task<bool> Authorize(UserLoginDto.Request requestBody);
    
    Task<bool> ReAuthorize(RefreshTokenDto.Request requestBody);

    void Logout();

    Task<TResponse?> SendRestApiRequest<TResponse>(
        RestApiEndpoint<TResponse> endpoint, IDictionary<string, string?>? query = null
    );

    Task<TResponse?> SendRestApiRequest<TRequest, TResponse>(
        RestApiEndpoint<TRequest, TResponse> endpoint, TRequest? requestBody = null,
        IDictionary<string, string?>? query = null
    ) where TRequest : class;

    Task<bool> SendRestApiRequest(RestApiEndpointNoContent endpoint, IDictionary<string, string?>? query = null);

    Task<bool> SendRestApiRequest<TRequest>(
        RestApiEndpointNoContent<TRequest> endpoint,
        TRequest? requestBody = null,
        IDictionary<string, string?>? query = null
    ) where TRequest : class;

    Task<bool> DownloadToFileAsync(RestApiEndpoint<bool> endpoint, string filepath);

    Task<TResponse?> SendMultipartRestApiRequest<TRequest, TResponse>(
        RestApiEndpoint<TRequest, TResponse> endpoint,
        TRequest? requestBody,
        IEnumerable<(string Name, string FileName, string ContentType, Stream Content)> files
    ) where TRequest : class;

    Task<bool> SendMultipartRestApiRequest<TRequest>(
        RestApiEndpointNoContent<TRequest> endpoint,
        TRequest? requestBody,
        IEnumerable<(string Name, string FileName, string ContentType, Stream Content)> files
    ) where TRequest : class;
}

public abstract class RestApiEndpoint(HttpMethod httpMethod, string endpoint, bool requiresAuth)
{
    public HttpMethod HttpMethod { get; init; } = httpMethod;
    public string Endpoint { get; init; } = endpoint;
    public bool RequiresAuth { get; init; } = requiresAuth;
}

public class RestApiEndpointNoContent(HttpMethod httpMethod, string endpoint, bool requiresAuth) :
    RestApiEndpoint(httpMethod, endpoint, requiresAuth) {}

public class RestApiEndpointNoContent<TRequest>(HttpMethod httpMethod, string endpoint, bool requiresAuth) :
    RestApiEndpoint(httpMethod, endpoint, requiresAuth) where TRequest : class {}

public class RestApiEndpoint<TResponse>(HttpMethod httpMethod, string endpoint, bool requiresAuth) : 
    RestApiEndpoint(httpMethod, endpoint, requiresAuth) {}

public class RestApiEndpoint<TRequest, TResponse>(HttpMethod httpMethod, string endpoint, bool requiresAuth) : 
    RestApiEndpoint(httpMethod, endpoint, requiresAuth) where TRequest : class {}

internal partial class RestApiEndpoints
{
    
}
