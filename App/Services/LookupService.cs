using App.Models;

namespace App.Services;

internal class LookupService(IRestApiService restApiService) : ILookupService
{
    private readonly IRestApiService _restService = restApiService;

    // list caches
    private readonly List<LookupItem>? _typesList = null;
    private readonly List<LookupItem>? _countriesList = null;
    private readonly List<LookupItem>? _statusesList = null;
    private readonly List<LookupItem>? _qualitiesList = null;
    private readonly List<LookupItem>? _specialStatusesList = null;

    private static (int, string) Extract(object dto)
    {
        var idProp = dto.GetType().GetProperty("Id");
        var nameProp = dto.GetType().GetProperty("Name");
        var id = (int) (idProp?.GetValue(dto) ?? 0);
        var name = (string?) nameProp?.GetValue(dto) ?? string.Empty;

        return (id, name);
    }

    // Lists for drop-down lists
    public Task<IReadOnlyList<LookupItem>> GetTypesAsync(CancellationToken ct = default)
        => GetListAsync(
            _typesList,
            RestApiEndpoints.GetCollectionItemTypes,
            "lastSeenId",
            static (id, name) => id
        );

    public Task<IReadOnlyList<LookupItem>> GetStatusesAsync(CancellationToken ct = default)
        => GetListAsync(
            _statusesList,
            RestApiEndpoints.GetCollectionItemStatuses,
            "lastSeenId",
            static (id, name) => id
        );

    public Task<IReadOnlyList<LookupItem>> GetQualitiesAsync(CancellationToken ct = default)
        => GetListAsync(
            _qualitiesList,
            RestApiEndpoints.GetCollectionItemQualities,
            "lastSeenId",
            static (id, name) => id
        );

    public Task<IReadOnlyList<LookupItem>> GetSpecialStatusesAsync(CancellationToken ct = default)
        => GetListAsync(
            _specialStatusesList,
            RestApiEndpoints.GetCollectionItemSpecialStatuses,
            "lastSeenId",
            static (id, name) => id
        );

    public Task<IReadOnlyList<LookupItem>> GetCountriesAsync(CancellationToken ct = default)
        => GetListAsync(
            _countriesList,
            RestApiEndpoints.GetCountries,
            "lastSeenName",
            static (id, name) => name
        );

    private async Task<IReadOnlyList<LookupItem>> GetListAsync<TCursor, TDto>(
        List<LookupItem>? cache,
        RestApiEndpoint<List<TDto>> endpoint,
        string cursorQueryKey,
        Func<int, string, TCursor?> cursorSelector) where TDto : class
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

                lastCursor = cursorSelector(id, name);
            }
        }

        return cache;
    }
}
