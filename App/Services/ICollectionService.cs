using App.Models;
using Shared.Models.Collection;

namespace App.Services;

public interface ICollectionService
{
    Task<IEnumerable<MyCollectionDto>> GetMyCollectionsAsync(Guid? lastSeenId, string? lastSeenName);

    Task<Guid?> CreateCollectionAsync(CollectionCreationDto.Request request);
}

internal partial class RestApiEndpoints
{
    public static readonly RestApiEndpoint<List<CollectionDto.Response>>
        GetMyCollections = new(HttpMethod.Get, "Collection/my", true);

    public static readonly RestApiEndpoint<CollectionCreationDto.Request, CollectionCreationDto.Response>
        CreateCollection = new(HttpMethod.Post, "Collection/create", true);
}
