using Shared.Models.CollectionItem;

namespace App.Services;

public interface ICollectionItemService
{
    Task<IEnumerable<CollectionItemDto.Response>> GetCollectionItemsAsync(Guid collectionId, int? lastSeenId);
}

internal partial class RestApiEndpoints
{
    public static RestApiEndpoint<List<CollectionItemDto.Response>> GetCollectionItems(Guid collectionId)
        => new(HttpMethod.Get, $"CollectionItem/{collectionId}", true);
}
