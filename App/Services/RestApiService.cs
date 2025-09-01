using Shared.Models.Auth;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace App.Services;

internal class RestApiService : IRestApiService
{
    private static Uri BaseUri => new (new (AppSettings.ServerUrl), "api/");

    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    private string? _authToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

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

    public async Task<bool> Authorize(UserLoginDto.Request requestBody)
    {
        RefreshTokenDto.Response? result = await SendInternalRestApiRequest(RestApiEndpoints.Login, requestBody);

        if (result != null)
        {
            _authToken = result.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(30); // TODO: receive expiry from server
            AppSettings.RefreshToken = result.RefreshToken;
            AppSettings.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // TODO: receive expiry from server
            AppSettings.Username = requestBody.Username;

            return true;
        }

        return false;
    }

    public async Task<bool> ReAuthorize(RefreshTokenDto.Request requestBody)
    {
        RefreshTokenDto.Response? result = await SendInternalRestApiRequest(RestApiEndpoints.ReLogin, requestBody);

        if (result != null)
        {
            _authToken = result.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(30); // TODO: receive expiry from server
            AppSettings.RefreshToken = result.RefreshToken;
            AppSettings.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // TODO: receive expiry from server

            return true;
        }

        return false;
    }

    public async Task<TResponse?> SendRestApiRequest<TResponse>(RestApiEndpoint<TResponse> endpoint)
    {
        if (IsTokenExpired)
        {
            bool authorized = await ReAuthorize(new RefreshTokenDto.Request
            {
                Username = AppSettings.Username,
                RefreshToken = AppSettings.RefreshToken
            });

            if (!authorized)
            {
                return default;
            }
        }

        return await SendInternalRestApiRequest(endpoint);
    }

    public async Task<TResponse?> SendRestApiRequest<TRequest, TResponse>(RestApiEndpoint<TRequest, TResponse> endpoint, TRequest? requestBody = null) where TRequest : class
    {
        if (IsTokenExpired)
        {
            bool authorized = await ReAuthorize(new RefreshTokenDto.Request
            { 
                Username = AppSettings.Username,
                RefreshToken = AppSettings.RefreshToken
            });

            if (!authorized)
            {
                return default;
            }
        }

        return await SendInternalRestApiRequest(endpoint, requestBody);
    }

    private async Task<TResponse?> SendInternalRestApiRequest<TResponse>(RestApiEndpoint<TResponse> endpoint)
    {
        Uri uri = new(BaseUri, endpoint.Endpoint);

        TResponse? result = default;
        try
        {
            HttpRequestMessage requestMessage = GenerateRequestMessage(endpoint.HttpMethod, uri);

            HttpResponseMessage response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);
            }
        }
        catch (Exception)
        {

        }

        return result;
    }

    private async Task<TResponse?> SendInternalRestApiRequest<TRequest, TResponse>(RestApiEndpoint<TRequest, TResponse> endpoint, TRequest? requestBody = null) where TRequest : class
    {
        Uri uri = new(BaseUri, endpoint.Endpoint);

        TResponse? result = default;
        try
        {
            HttpRequestMessage requestMessage;
            if (requestBody is not null)
            {
                string json = JsonSerializer.Serialize(requestBody, _serializerOptions);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                requestMessage = GenerateRequestMessage(endpoint.HttpMethod, uri, requestContent);
            }
            else
            {
                requestMessage = GenerateRequestMessage(endpoint.HttpMethod, uri);
            }

            HttpResponseMessage response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);
            }
        }
        catch (Exception)
        {

        }

        return result;
    }

    private HttpRequestMessage GenerateRequestMessage(HttpMethod httpMethod, Uri uri, HttpContent? content = null)
    {
        var message = new HttpRequestMessage(httpMethod, uri)
        {
            Content = content,
        };

        message.Headers.Add("Accept", "application/json");
        if (!string.IsNullOrEmpty(_authToken))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }

        return message;
    }

    public void Logout()
    {
        _authToken = null;
        _tokenExpiry = DateTime.MinValue;
        AppSettings.Username = string.Empty;
        AppSettings.RefreshToken = string.Empty;
        AppSettings.RefreshTokenExpiry = null;
    }

    private bool IsTokenExpired => DateTime.UtcNow >= _tokenExpiry;
}
