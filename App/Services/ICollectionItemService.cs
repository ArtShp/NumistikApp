using App.Models;

namespace App.Services;

public interface ICollectionItemService
{
    Task<IEnumerable<CollectionItemPreview>> GetCollectionItemsAsync(Guid collectionId, int? lastSeenId);
}

internal partial class RestApiEndpoints
{
    public static RestApiEndpoint<List<CollectionItemPreview>> GetCollectionItems(Guid collectionId)
        => new(HttpMethod.Get, $"CollectionItem/{collectionId}", true);
}
