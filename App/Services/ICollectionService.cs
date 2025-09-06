using App.Models;

namespace App.Services;

public interface ICollectionService
{
    Task<List<MyCollectionDto>> GetMyCollectionsAsync(Guid? lastSeenId, string? lastSeenName);
}

internal partial class RestApiEndpoints
{
    public static readonly RestApiEndpoint<List<MyCollectionDto>>
        GetMyCollections = new(HttpMethod.Get, "Collection/my", true);
}
