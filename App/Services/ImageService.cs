using System.Security.Cryptography;
using System.Text;

namespace App.Services;

internal class ImageService(IRestApiService restApiService) : IImageService
{
    private readonly IRestApiService _restApiService = restApiService;

    public async Task<string?> GetLocalPathAsync(string? fileUrl, bool forceRefresh = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return null;

        var cacheDir = GetImagesCacheDir();
        Directory.CreateDirectory(cacheDir);

        string extension = Path.GetExtension(fileUrl);
        if (string.IsNullOrEmpty(extension)) return null;

        var localFile = Path.Combine(cacheDir, HashToFileName(fileUrl, extension));

        if (!forceRefresh && File.Exists(localFile))
        {
            try
            {
                var info = new FileInfo(localFile);
                if (info.Length > 0) return localFile;
            }
            catch
            {

            }
        }

        bool ok = await _restApiService.DownloadToFileAsync(RestApiEndpoints.GetImage(fileUrl), localFile);
        return ok ? localFile : null;
    }

    public Task ClearCacheAsync(TimeSpan? maxAge = null, CancellationToken ct = default)
    {
        try
        {
            var dir = GetImagesCacheDir();
            if (!Directory.Exists(dir)) return Task.CompletedTask;

            foreach (var file in Directory.EnumerateFiles(dir))
            {
                if (ct.IsCancellationRequested) break;

                if (maxAge is null)
                {
                    File.Delete(file);
                    continue;
                }

                var info = new FileInfo(file);
                var age = DateTime.UtcNow - (info.LastWriteTimeUtc);
                if (age >= maxAge.Value)
                {
                    File.Delete(file);
                }
            }
        }
        catch
        {

        }

        return Task.CompletedTask;
    }

    private static string HashToFileName(string key, string extension)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        var hash = Convert.ToHexString(bytes);

        return $"{hash}{extension}";
    }

    private static string GetImagesCacheDir()
        => Path.Combine(FileSystem.CacheDirectory, "images");
}

internal static partial class RestApiEndpoints
{
    public static RestApiEndpoint<bool> GetImage(string filename) =>
        new(HttpMethod.Get, $"CollectionItem/image/{filename}", true);
}
