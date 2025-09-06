using Shared.Models.CollectionItem;

namespace App.Services;

internal class CollectionItemService(IRestApiService restApiService) : ICollectionItemService
{
    private readonly IRestApiService _restApiService = restApiService;

    public async Task<IEnumerable<CollectionItemDto.Response>> GetCollectionItemsAsync(Guid collectionId, int? lastSeenId)
    {
        var endpoint = RestApiEndpoints.GetCollectionItems(collectionId);
        var query = new Dictionary<string, string?> { ["lastSeenId"] = lastSeenId?.ToString() };

        var result = await _restApiService.SendRestApiRequest(endpoint, query);
        return result ?? [];
    }
}
