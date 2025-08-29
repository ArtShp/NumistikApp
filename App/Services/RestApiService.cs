using System.Text.Json;

namespace App.Services;

internal class RestApiService : IRestApiService
{
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
