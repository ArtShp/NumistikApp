using Common;
using Shared.Models.Auth;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace App.Services;

internal class RestApiService : IRestApiService
{
    private static Uri BaseUri => new(new(AppSettings.ServerUrl), "api/");

    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    private string? _authToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

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
            _tokenExpiry = DateTime.UtcNow.Add(Settings.AccessTokenExpiration);
            AppSettings.RefreshToken = result.RefreshToken;
            AppSettings.RefreshTokenExpiry = DateTime.UtcNow.Add(Settings.RefreshTokenExpiration);
            AppSettings.Username = requestBody.Username;

            return true;
        }

        return false;
    }

    public async Task<bool> ReAuthorize(RefreshTokenDto.Request requestBody)
    {
        if (IsRefreshTokenExpired)
            return false;

        if (!IsTokenExpired && _authToken is not null)
            return true;

        await _refreshSemaphore.WaitAsync();
        try
        {
            if (IsRefreshTokenExpired)
                return false;

            if (!IsTokenExpired && _authToken is not null)
                return true;

            RefreshTokenDto.Response? result = await SendInternalRestApiRequest(RestApiEndpoints.ReLogin, requestBody);

            if (result != null)
            {
                _authToken = result.AccessToken;
                _tokenExpiry = DateTime.UtcNow.Add(Settings.AccessTokenExpiration);
                AppSettings.RefreshToken = result.RefreshToken;
                AppSettings.RefreshTokenExpiry = DateTime.UtcNow.Add(Settings.RefreshTokenExpiration);

                return true;
            }

            return false;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }

    public async Task<TResponse?> SendRestApiRequest<TResponse>(RestApiEndpoint<TResponse> endpoint, IDictionary<string, string?>? query = null)
    {
        if (endpoint.RequiresAuth && IsTokenExpired)
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

        return await SendInternalRestApiRequest(endpoint, query);
    }

    public async Task<TResponse?> SendRestApiRequest<TRequest, TResponse>(RestApiEndpoint<TRequest, TResponse> endpoint, TRequest? requestBody = null,
        IDictionary<string, string?>? query = null) where TRequest : class
    {
        if (endpoint.RequiresAuth && IsTokenExpired)
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

        return await SendInternalRestApiRequest(endpoint, requestBody, query);
    }

    public async Task<bool> SendRestApiRequest(RestApiEndpointNoContent endpoint, IDictionary<string, string?>? query = null)
    {
        var uriBuilder = new UriBuilder(new Uri(BaseUri, endpoint.Endpoint));

        if (query is not null && query.Count > 0)
        {
            var q = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var kv in query)
            {
                if (!string.IsNullOrWhiteSpace(kv.Value))
                {
                    q[kv.Key] = kv.Value;
                }
            }

            uriBuilder.Query = q.ToString();
        }

        Uri uri = uriBuilder.Uri;

        bool result = false;
        try
        {
            HttpRequestMessage requestMessage = GenerateRequestMessage(endpoint.HttpMethod, uri);

            HttpResponseMessage response = await _client.SendAsync(requestMessage);

            result = response.IsSuccessStatusCode;
        }
        catch (Exception)
        {

        }

        return result;
    }

    public async Task<bool> SendRestApiRequest<TRequest>(RestApiEndpointNoContent<TRequest> endpoint, TRequest? requestBody = null,
        IDictionary<string, string?>? query = null) where TRequest : class
    {
        if (endpoint.RequiresAuth && IsTokenExpired)
        {
            bool authorized = await ReAuthorize(new RefreshTokenDto.Request
            {
                Username = AppSettings.Username,
                RefreshToken = AppSettings.RefreshToken
            });

            if (!authorized)
            {
                return false;
            }
        }

        var uriBuilder = new UriBuilder(new Uri(BaseUri, endpoint.Endpoint));

        if (query is not null && query.Count > 0)
        {
            var q = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var kv in query)
            {
                if (!string.IsNullOrWhiteSpace(kv.Value))
                {
                    q[kv.Key] = kv.Value;
                }
            }

            uriBuilder.Query = q.ToString();
        }

        Uri uri = uriBuilder.Uri;

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
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<TResponse?> SendInternalRestApiRequest<TResponse>(RestApiEndpoint<TResponse> endpoint, IDictionary<string, string?>? query = null)
    {
        var uriBuilder = new UriBuilder(new Uri(BaseUri, endpoint.Endpoint));

        if (query is not null && query.Count > 0)
        {
            var q = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var kv in query)
            {
                if (!string.IsNullOrWhiteSpace(kv.Value))
                {
                    q[kv.Key] = kv.Value;
                }
            }

            uriBuilder.Query = q.ToString();
        }

        Uri uri = uriBuilder.Uri;

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

    private async Task<TResponse?> SendInternalRestApiRequest<TRequest, TResponse>(RestApiEndpoint<TRequest, TResponse> endpoint, TRequest? requestBody = null,
        IDictionary<string, string?>? query = null) where TRequest : class
    {
        var uriBuilder = new UriBuilder(new Uri(BaseUri, endpoint.Endpoint));

        if (query is not null && query.Count > 0)
        {
            var q = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var kv in query)
            {
                if (!string.IsNullOrWhiteSpace(kv.Value))
                {
                    q[kv.Key] = kv.Value;
                }
            }

            uriBuilder.Query = q.ToString();
        }

        Uri uri = uriBuilder.Uri;

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

    public async Task<TResponse?> SendMultipartRestApiRequest<TRequest, TResponse>(
        RestApiEndpoint<TRequest, TResponse> endpoint,
        TRequest? requestBody,
        IEnumerable<(string Name, string FileName, string ContentType, Stream Content)> files
    ) where TRequest : class
    {
        if (endpoint.RequiresAuth && IsTokenExpired)
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

        Uri uri = new(BaseUri, endpoint.Endpoint);

        TResponse? result = default;
        try
        {
            using var form = new MultipartFormDataContent();

            if (requestBody is not null)
            {
                string json = JsonSerializer.Serialize(requestBody, _serializerOptions);
                var fields = JsonSerializer.Deserialize<Dictionary<string, object?>>(json, _serializerOptions);

                if (fields is not null)
                {
                    foreach (var kv in fields)
                    {
                        var value = kv.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            form.Add(new StringContent(value), kv.Key);
                        }
                    }
                }
            }

            foreach (var (Name, FileName, ContentType, Content) in files)
            {
                var streamContent = new StreamContent(Content);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

                form.Add(streamContent, Name, FileName);
            }

            using var request = GenerateRequestMessage(endpoint.HttpMethod, uri, form);
            using var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);
            }
        }
        catch
        {

        }
        finally
        {
            foreach (var (_, _, _, Content) in files)
            {
                try { Content.Dispose(); } catch { }
            }
        }

        return result;
    }

    public async Task<bool> SendMultipartRestApiRequest<TRequest>(
        RestApiEndpointNoContent<TRequest> endpoint,
        TRequest? requestBody,
        IEnumerable<(string Name, string FileName, string ContentType, Stream Content)> files
    ) where TRequest : class
    {
        if (endpoint.RequiresAuth && IsTokenExpired)
        {
            bool authorized = await ReAuthorize(new RefreshTokenDto.Request
            {
                Username = AppSettings.Username,
                RefreshToken = AppSettings.RefreshToken
            });

            if (!authorized)
            {
                return false;
            }
        }

        Uri uri = new(BaseUri, endpoint.Endpoint);

        try
        {
            using var form = new MultipartFormDataContent();

            if (requestBody is not null)
            {
                string json = JsonSerializer.Serialize(requestBody, _serializerOptions);
                var fields = JsonSerializer.Deserialize<Dictionary<string, object?>>(json, _serializerOptions);

                if (fields is not null)
                {
                    foreach (var kv in fields)
                    {
                        var value = kv.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            form.Add(new StringContent(value), kv.Key);
                        }
                    }
                }
            }

            foreach (var (Name, FileName, ContentType, Content) in files)
            {
                var streamContent = new StreamContent(Content);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

                form.Add(streamContent, Name, FileName);
            }

            using var request = GenerateRequestMessage(HttpMethod.Post, uri, form);
            using var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
        finally
        {
            foreach (var (_, _, _, Content) in files)
            {
                try { Content.Dispose(); } catch { }
            }
        }
    }

    public async Task<bool> DownloadToFileAsync(RestApiEndpoint<bool> endpoint, string filepath)
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
                return false;
            }
        }

        Uri uri = new(BaseUri, endpoint.Endpoint);

        try
        {
            using var request = GenerateRequestMessage(endpoint.HttpMethod, uri);
            using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
                return false;

            var dir = Path.GetDirectoryName(filepath)!;
            Directory.CreateDirectory(dir);

            var temp = Path.Combine(dir, Guid.NewGuid().ToString("N") + ".tmp");
            await using (var input = await response.Content.ReadAsStreamAsync())
            await using (var output = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await input.CopyToAsync(output);
            }

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            File.Move(temp, filepath);
            return true;
        }
        catch
        {
            return false;
        }
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

    private static bool IsRefreshTokenExpired => DateTime.UtcNow >= (AppSettings.RefreshTokenExpiry ?? DateTime.MinValue);
}
