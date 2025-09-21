using App.Models;
using Shared.Models.CollectionItemQuality;
using Shared.Models.CollectionItemSpecialStatus;
using Shared.Models.CollectionItemStatus;
using Shared.Models.CollectionItemType;
using Shared.Models.Country;

namespace App.Services;

public interface ILookupService
{
    Task<IReadOnlyList<LookupItem>> GetTypesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetCountriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetStatusesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetQualitiesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LookupItem>> GetSpecialStatusesAsync(CancellationToken ct = default);

    Task<string?> GetTypeNameAsync(int id, CancellationToken ct = default);
    Task<string?> GetCountryNameAsync(int id, CancellationToken ct = default);
    Task<string?> GetStatusNameAsync(int id, CancellationToken ct = default);
    Task<string?> GetQualityNameAsync(int id, CancellationToken ct = default);
    Task<string?> GetSpecialStatusNameAsync(int id, CancellationToken ct = default);
}

internal partial class RestApiEndpoints
{
    public static RestApiEndpoint<List<CollectionItemTypeDto.Response>> GetCollectionItemTypes
        => new(HttpMethod.Get, "CollectionItemType", true);

    public static RestApiEndpoint<CollectionItemTypeDto.Response> GetCollectionItemType(int id)
        => new(HttpMethod.Get, $"CollectionItemType/{id}", true);

    public static RestApiEndpoint<List<CollectionItemStatusDto.Response>> GetCollectionItemStatuses
        => new(HttpMethod.Get, "CollectionItemStatus", true);

    public static RestApiEndpoint<CollectionItemStatusDto.Response> GetCollectionItemStatus(int id)
        => new(HttpMethod.Get, $"CollectionItemStatus/{id}", true);

    public static RestApiEndpoint<List<CollectionItemQualityDto.Response>> GetCollectionItemQualities
        => new(HttpMethod.Get, "CollectionItemQuality", true);

    public static RestApiEndpoint<CollectionItemQualityDto.Response> GetCollectionItemQuality(int id)
        => new(HttpMethod.Get, $"CollectionItemQuality/{id}", true);

    public static RestApiEndpoint<List<CollectionItemSpecialStatusDto.Response>> GetCollectionItemSpecialStatuses
        => new(HttpMethod.Get, "CollectionItemSpecialStatus", true);

    public static RestApiEndpoint<CollectionItemSpecialStatusDto.Response> GetCollectionItemSpecialStatus(int id)
        => new(HttpMethod.Get, $"CollectionItemSpecialStatus/{id}", true);

    public static RestApiEndpoint<List<CountryDto.Response>> GetCountries
        => new(HttpMethod.Get, "Country", true);

    public static RestApiEndpoint<CountryDto.Response> GetCountry(int id)
        => new(HttpMethod.Get, $"Country/{id}", true);
}
