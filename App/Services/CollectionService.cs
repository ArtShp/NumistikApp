using App.Models;
using Shared.Models.Collection;
using Shared.Models.Common;

namespace App.Services;

internal class CollectionService(IRestApiService restApiService) : ICollectionService
{
    private readonly IRestApiService _restApiService = restApiService;

    public async Task<IEnumerable<MyCollectionDto>> GetMyCollectionsAsync(Guid? lastSeenId, string? lastSeenName)
    {
        var query = new Dictionary<string, string?>
        {
            ["lastSeenId"] = lastSeenId?.ToString(),
            ["lastSeenName"] = lastSeenName
        };

        var result = await _restApiService.SendRestApiRequest(RestApiEndpoints.GetMyCollections, query);

        if (result is null) return [];

        return result.Select(item => new MyCollectionDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            CollectionRole = item.CollectionRole!.Value
        }
        );
    }

    public async Task<Guid?> CreateCollectionAsync(CollectionCreationDto.Request request)
    {
        var result = await _restApiService.SendRestApiRequest(
            RestApiEndpoints.CreateCollection, request
        );

        return result?.Id;
    }

    public async Task<IReadOnlyList<CollectionMemberDto>> GetCollectionMembersAsync(Guid collectionId)
    {
        var endpoint = RestApiEndpoints.GetCollectionMembers(collectionId);

        var result = await _restApiService.SendRestApiRequest(endpoint, null);
        if (result is null || result.Members is null) return [];

        return result.Members
            .Select(m => new CollectionMemberDto
            {
                UserId = m.UserId,
                Username = m.Username,
                Role = m.Role
            })
            .ToList();
    }

    public async Task<bool> UpdateCollectionRoleAsync(Guid collectionId, Guid userId, CollectionRole role)
    {
        var request = new CollectionUpdateRoleDto.Request
        {
            CollectionId = collectionId,
            UserId = userId,
            Role = role
        };

        var ok = await _restApiService.SendRestApiRequest(RestApiEndpoints.UpdateCollectionRole, request);

        return ok;
    }

    public async Task<IReadOnlyList<CollectionRole>> GetAssignableRolesAsync(Guid collectionId)
    {
        var endpoint = RestApiEndpoints.GetAssignableRoles(collectionId);

        var result = await _restApiService.SendRestApiRequest(endpoint, null);

        return result ?? [];
    }
}
