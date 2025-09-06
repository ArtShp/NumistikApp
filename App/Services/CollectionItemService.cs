using App.Models;

namespace App.Services;

internal class CollectionItemService(IRestApiService restApiService) : ICollectionItemService
{
    private readonly IRestApiService _restApiService = restApiService;

    public async Task<IEnumerable<CollectionItemPreview>> GetCollectionItemsAsync(Guid collectionId, int? lastSeenId)
    {
        var endpoint = RestApiEndpoints.GetCollectionItems(collectionId);
        var query = new Dictionary<string, string?> { ["lastSeenId"] = lastSeenId?.ToString() };

        var result = await _restApiService.SendRestApiRequest(endpoint, query);

        if (result is null) return [];

        return result.Select(item => new CollectionItemPreview
        {
            Id = item.Id,
            CollectionId = item.CollectionId,
            Value = item.Value,
            Currency = item.Currency,
            SerialNumber = item.SerialNumber,
            Description = item.Description,
            ObverseImageUrl = item.ObverseImageUrl
        });
    }
}
