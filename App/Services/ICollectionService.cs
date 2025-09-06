using App.Models;
using Shared.Models.Collection;

namespace App.Services;

public interface ICollectionService
{
    Task<IEnumerable<MyCollectionDto>> GetMyCollectionsAsync(Guid? lastSeenId, string? lastSeenName);
}

internal partial class RestApiEndpoints
{
    public static readonly RestApiEndpoint<List<CollectionDto.Response>>
        GetMyCollections = new(HttpMethod.Get, "Collection/my", true);
}
