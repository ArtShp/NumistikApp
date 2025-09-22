using Shared;
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
        RefreshTokenDto.Response? result = await SendRestApiRequest(RestApiEndpoints.Login, requestBody);

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

        // Ensure only one refresh operation at a time
        await _refreshSemaphore.WaitAsync();
        try
        {
            if (IsRefreshTokenExpired)
                return false;

            if (!IsTokenExpired && _authToken is not null)
                return true;

            RefreshTokenDto.Response? result = await SendRestApiRequest(RestApiEndpoints.ReLogin, requestBody);

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

    public async Task<TResponse?> SendRestApiRequest<TRequest, TResponse>(
        RestApiEndpoint<TRequest, TResponse> endpoint,
        TRequest? requestBody = null,
        IDictionary<string, string?>? query = null
    ) where TRequest : class
    {
        var (ok, content) = await SendAsync(endpoint, requestBody, query);

        if (!ok || string.IsNullOrWhiteSpace(content))
            return default;

        try
        {
            return JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);
        }
        catch
        {
            return default;
        }
    }

    public async Task<bool> SendRestApiRequest<TRequest>(
        RestApiEndpoint<TRequest, Null> endpoint,
        TRequest? requestBody = null,
        IDictionary<string, string?>? query = null
    ) where TRequest : class
    {
        var (ok, _) = await SendAsync(endpoint, requestBody, query);
        return ok;
    }

    public async Task<TResponse?> SendMultipartRestApiRequest<TRequest, TResponse>(
        RestApiEndpoint<TRequest, TResponse> endpoint,
        TRequest? requestBody,
        IEnumerable<(string Name, string FileName, string ContentType, Stream Content)> files
    ) where TRequest : class
    {
        using var form = BuildMultipartContent(requestBody, files);

        var (ok, content) = await SendAsync(endpoint, requestBody, null, overrideContent: form);

        if (!ok || string.IsNullOrWhiteSpace(content))
            return default;

        try
        {
            return JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);
        }
        catch
        {
            return default;
        }
        finally
        {
            foreach (var (_, _, _, Content) in files)
            {
                try { Content.Dispose(); } catch { }
            }
        }
    }

    public async Task<bool> SendMultipartRestApiRequest<TRequest>(
        RestApiEndpoint<TRequest, Null> endpoint,
        TRequest? requestBody,
        IEnumerable<(string Name, string FileName, string ContentType, Stream Content)> files
    ) where TRequest : class
    {
        try
        {
            using var form = BuildMultipartContent(requestBody, files);
            var (ok, content) = await SendAsync(endpoint, requestBody, null, overrideContent: form);

            return ok;
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

    public async Task<bool> DownloadToFileAsync(RestApiEndpoint<Null, Null> endpoint, string filepath)
    {
        if (!await EnsureAuthAsync(endpoint.RequiresAuth))
            return false;

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

    private async Task<(bool ok, string? content)> SendAsync<TRequest, TResponse>(
        RestApiEndpoint<TRequest, TResponse> endpoint,
        TRequest? requestBody = null,
        IDictionary<string, string?>? query = null,
        HttpContent? overrideContent = null
    ) where TRequest : class
    {
        if (!await EnsureAuthAsync(endpoint.RequiresAuth))
            return (false, null);

        var uri = BuildUri(endpoint.Endpoint, query);

        try
        {
            HttpContent? content = overrideContent ?? BuildJsonContentIfAny(requestBody);

            using var request = GenerateRequestMessage(endpoint.HttpMethod, uri, content);
            using var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return (false, null);

            string? text = null;
            if (typeof(TResponse) != typeof(Null))
            {
                text = await response.Content.ReadAsStringAsync();
            }

            return (true, text);
        }
        catch
        {
            return (false, null);
        }
    }

    private async Task<bool> EnsureAuthAsync(bool requiresAuth)
    {
        if (!requiresAuth)
            return true;

        if (!IsTokenExpired && _authToken is not null)
            return true;

        return await ReAuthorize(new RefreshTokenDto.Request
        {
            Username = AppSettings.Username,
            RefreshToken = AppSettings.RefreshToken
        });
    }

    private static Uri BuildUri(string endpoint, IDictionary<string, string?>? query)
    {
        var uriBuilder = new UriBuilder(new Uri(BaseUri, endpoint));

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

        return uriBuilder.Uri;
    }

    private StringContent? BuildJsonContentIfAny<TRequest>(TRequest? requestBody) where TRequest : class
    {
        if (requestBody is null || typeof(TRequest) == typeof(Null))
            return null;

        string json = JsonSerializer.Serialize(requestBody, _serializerOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private MultipartFormDataContent BuildMultipartContent<TRequest>(
        TRequest? requestBody,
        IEnumerable<(string Name, string FileName, string ContentType, Stream Content)> files
    ) where TRequest : class
    {
        var form = new MultipartFormDataContent();

        if (requestBody is not null && typeof(TRequest) != typeof(Null))
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

        return form;
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
