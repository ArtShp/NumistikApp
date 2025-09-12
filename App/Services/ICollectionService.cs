using App.Models;
using Shared.Models.Collection;
using Shared.Models.Common;

namespace App.Services;

public interface ICollectionService
{
    Task<IEnumerable<MyCollectionDto>> GetMyCollectionsAsync(Guid? lastSeenId, string? lastSeenName);

    Task<Guid?> CreateCollectionAsync(CollectionCreationDto.Request request);

    Task<IReadOnlyList<CollectionMemberDto>> GetCollectionMembersAsync(Guid collectionId);

    Task<bool> UpdateCollectionRoleAsync(Guid collectionId, Guid userId, CollectionRole role);

    Task<IReadOnlyList<CollectionRole>> GetAssignableRolesAsync(Guid collectionId);

    Task<bool> AssignCollectionRoleAsync(Guid collectionId, string username, CollectionRole role);
}

internal partial class RestApiEndpoints
{
    public static readonly RestApiEndpoint<List<CollectionDto.Response>>
        GetMyCollections = new(HttpMethod.Get, "Collection/my", true);

    public static readonly RestApiEndpoint<CollectionCreationDto.Request, CollectionCreationDto.Response>
        CreateCollection = new(HttpMethod.Post, "Collection/create", true);

    public static RestApiEndpoint<CollectionMembersDto.Response> GetCollectionMembers(Guid collectionId)
        => new(HttpMethod.Get, $"Collection/{collectionId}/members", true);

    public static readonly RestApiEndpointNoContent<CollectionUpdateRoleDto.Request>
        UpdateCollectionRole = new(HttpMethod.Post, "Collection/role", true);

    public static RestApiEndpoint<List<CollectionRole>> GetAssignableRoles(Guid collectionId)
        => new(HttpMethod.Get, $"Collection/{collectionId}/assignable-roles", true);
}
