using App.Models;
using Shared.Models.CollectionItem;

namespace App.Services;

public interface ICollectionItemService
{
    Task<IEnumerable<CollectionItemPreview>> GetCollectionItemsAsync(Guid collectionId, int? lastSeenId);
}

internal partial class RestApiEndpoints
{
    public static RestApiEndpoint<List<CollectionItemDto.Response>> GetCollectionItems(Guid collectionId)
        => new(HttpMethod.Get, $"CollectionItem/{collectionId}", true);
}
