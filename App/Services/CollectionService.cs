using App.Models;
using Shared.Models.Collection;

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
}
