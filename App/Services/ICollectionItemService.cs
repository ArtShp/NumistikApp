using App.Models;
using Shared.Models.CollectionItem;

namespace App.Services;

public interface ICollectionItemService
{
    Task<IEnumerable<CollectionItem>> GetCollectionItemsAsync(Guid collectionId, int? lastSeenId);
    Task<CollectionItem?> GetCollectionItemAsync(Guid collectionId, int itemId);
    Task<int?> CreateCollectionItemAsync(CollectionItemCreateRequest request);
    Task<bool> UpdateCollectionItemAsync(CollectionItemUpdateRequest request);
    Task<bool> DeleteCollectionItemAsync(Guid collectionId, int itemId);
}

internal partial class RestApiEndpoints
{
    public static RestApiEndpoint<List<CollectionItemDto.Response>> GetCollectionItems(Guid collectionId)
        => new(HttpMethod.Get, $"CollectionItem/{collectionId}", true);

    public static RestApiEndpoint<CollectionItemDto.Response> GetCollectionItem(Guid collectionId, int itemId)
        => new(HttpMethod.Get, $"CollectionItem/{collectionId}/{itemId}", true);

    public static RestApiEndpoint<CollectionItemCreationDto.Request, CollectionItemCreationDto.Response> CreateCollectionItem
        => new(HttpMethod.Post, "CollectionItem/create", true);

    public static RestApiEndpointNoContent<CollectionItemUpdateDto.Request> UpdateCollectionItem
        => new(HttpMethod.Post, "CollectionItem/update", true);

    public static RestApiEndpointNoContent DeleteCollectionItem(Guid collectionId, int itemId)
        => new(HttpMethod.Delete, $"CollectionItem/{collectionId}/{itemId}", true);
}
