using App.Models;
using System.Collections.Concurrent;

namespace App.Services;

internal class LookupService(IRestApiService restApiService) : ILookupService
{
    private readonly IRestApiService _restService = restApiService;

    private readonly List<LookupItem>? _typesList = null;
    private readonly List<LookupItem>? _countriesList = null;
    private readonly List<LookupItem>? _statusesList = null;
    private readonly List<LookupItem>? _qualitiesList = null;
    private readonly List<LookupItem>? _specialStatusesList = null;

    private readonly ConcurrentDictionary<int, string> _typeNameCache = new();
    private readonly ConcurrentDictionary<int, string> _countryNameCache = new();
    private readonly ConcurrentDictionary<int, string> _statusNameCache = new();
    private readonly ConcurrentDictionary<int, string> _qualityNameCache = new();
    private readonly ConcurrentDictionary<int, string> _specialStatusNameCache = new();

    private static (int, string) Extract(object dto)
    {
        var idProp = dto.GetType().GetProperty("Id");
        var nameProp = dto.GetType().GetProperty("Name");
        var id = (int)(idProp?.GetValue(dto) ?? 0);
        var name = (string?)nameProp?.GetValue(dto) ?? string.Empty;

        return (id, name);
    }

    private static (int, string)? TryExtract(object? dto)
    {
        if (dto is null) return null;

        var idProp = dto.GetType().GetProperty("Id");
        var nameProp = dto.GetType().GetProperty("Name");
        if (idProp is null || nameProp is null) return null;

        var idObj = idProp.GetValue(dto);
        var nameObj = nameProp.GetValue(dto);

        if (idObj is not int id) return null;
        if (nameObj is not string name) return null;

        return (id, name);
    }

    public Task<IReadOnlyList<LookupItem>> GetTypesAsync(CancellationToken ct = default)
        => GetListAsync(
            _typesList,
            RestApiEndpoints.GetCollectionItemTypes,
            "lastSeenId",
            static (id, name) => id,
            cacheWriter: (id, name) => _typeNameCache.TryAdd(id, name)
        );

    public Task<string?> GetTypeNameAsync(int id, CancellationToken ct = default)
        => GetNameAsync(id, _typeNameCache, RestApiEndpoints.GetCollectionItemType);

    public Task<IReadOnlyList<LookupItem>> GetStatusesAsync(CancellationToken ct = default)
        => GetListAsync(
            _statusesList,
            RestApiEndpoints.GetCollectionItemStatuses,
            "lastSeenId",
            static (id, name) => id,
            cacheWriter: (id, name) => _statusNameCache.TryAdd(id, name)
        );

    public Task<string?> GetStatusNameAsync(int id, CancellationToken ct = default)
        => GetNameAsync(id, _statusNameCache, RestApiEndpoints.GetCollectionItemStatus);

    public Task<IReadOnlyList<LookupItem>> GetQualitiesAsync(CancellationToken ct = default)
        => GetListAsync(
            _qualitiesList,
            RestApiEndpoints.GetCollectionItemQualities,
            "lastSeenId",
            static (id, name) => id,
            cacheWriter: (id, name) => _qualityNameCache.TryAdd(id, name)
        );

    public Task<string?> GetQualityNameAsync(int id, CancellationToken ct = default)
        => GetNameAsync(id, _qualityNameCache, RestApiEndpoints.GetCollectionItemQuality);

    public Task<IReadOnlyList<LookupItem>> GetSpecialStatusesAsync(CancellationToken ct = default)
        => GetListAsync(
            _specialStatusesList,
            RestApiEndpoints.GetCollectionItemSpecialStatuses,
            "lastSeenId",
            static (id, name) => id,
            cacheWriter: (id, name) => _specialStatusNameCache.TryAdd(id, name)
        );

    public Task<string?> GetSpecialStatusNameAsync(int id, CancellationToken ct = default)
        => GetNameAsync(id, _specialStatusNameCache, RestApiEndpoints.GetCollectionItemSpecialStatus);

    public Task<IReadOnlyList<LookupItem>> GetCountriesAsync(CancellationToken ct = default)
        => GetListAsync(
            _countriesList,
            RestApiEndpoints.GetCountries,
            "lastSeenName",
            static (id, name) => name,
            cacheWriter: (id, name) => _countryNameCache.TryAdd(id, name)
        );

    public Task<string?> GetCountryNameAsync(int id, CancellationToken ct = default)
        => GetNameAsync(id, _countryNameCache, RestApiEndpoints.GetCountry);

    private async Task<IReadOnlyList<LookupItem>> GetListAsync<TCursor, TDto>(
        List<LookupItem>? cache,
        RestApiEndpoint<List<TDto>> endpoint,
        string cursorQueryKey,
        Func<int, string, TCursor?> cursorSelector,
        Action<int, string>? cacheWriter = null) where TDto : class
    {
        if (cache is not null && cache.Count > 0) return cache;

        cache = [];

        TCursor? lastCursor = default;
        const int maxPages = 100; // for unexpected large lists, avoid infinite loops

        for (int iter = 0; iter < maxPages; ++iter)
        {
            var page = await _restService.SendRestApiRequest(endpoint, new Dictionary<string, string?>
            {
                [cursorQueryKey] = lastCursor?.ToString()
            });

            if (page is null || page.Count == 0)
                break;

            foreach (var dto in page)
            {
                var (id, name) = Extract(dto!);

                cache.Add(new LookupItem { Id = id, Name = name });

                cacheWriter?.Invoke(id, name);
                lastCursor = cursorSelector(id, name);
            }
        }

        return cache;
    }

    private async Task<string?> GetNameAsync<TResponse>(
        int id,
        ConcurrentDictionary<int, string> cache,
        Func<int, RestApiEndpoint<TResponse>> endpointFactory)
    {
        if (id <= 0) return null;
        if (cache.TryGetValue(id, out var cached)) return cached;

        var dto = await _restService.SendRestApiRequest(endpointFactory(id));
        var extracted = TryExtract(dto);

        if (extracted is { } e)
        {
            cache.TryAdd(e.Item1, e.Item2);
            return e.Item2;
        }

        return null;
    }
}
