using System.Text.Json;

namespace App.Services;

internal class RestApiService : IRestApiService
{
    private static Uri BaseUri => new (new (AppSettings.ServerUrl), "api/");

    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    public RestApiService()
    {
#if DEBUG
        HttpClientHandler insecureHandler = GetInsecureHandler();
        _client = new HttpClient(insecureHandler);
#else
        _client = new HttpClient();
#endif
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    private static HttpClientHandler GetInsecureHandler()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;

                return errors == System.Net.Security.SslPolicyErrors.None;
            }
        };

        return handler;
    }
}

internal class RestApiEndpoints
{
    
}

internal class RestApiEndpoint<TRequest, TResponse>(HttpMethod httpMethod, string endpoint)
{
    public HttpMethod HttpMethod { get; init; } = httpMethod;
    public string Endpoint { get; init; } = endpoint;
}
