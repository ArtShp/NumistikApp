using App.Models;

namespace App.Services;

internal class CollectionService(IRestApiService restApiService) : ICollectionService
{
    private readonly IRestApiService _restApiService = restApiService;

    public async Task<List<MyCollectionDto>> GetMyCollectionsAsync(Guid? lastSeenId, string? lastSeenName)
    {
        var query = new Dictionary<string, string?>
        {
            ["lastSeenId"] = lastSeenId?.ToString(),
            ["lastSeenName"] = lastSeenName
        };

        var result = await _restApiService.SendRestApiRequest(RestApiEndpoints.GetMyCollections, query);

        return result ?? [];
    }
}
