namespace App.Services;

public interface IImageService
{
    Task<string?> GetLocalPathAsync(string? fileNameOrUrl, bool forceRefresh = false, CancellationToken ct = default);

    Task ClearCacheAsync(TimeSpan? maxAge = null, CancellationToken ct = default);
}
