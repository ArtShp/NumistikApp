using App.Models;
using Shared.Models.CollectionItem;

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

    public async Task<int?> CreateCollectionItemAsync(CollectionItemCreateRequest request)
    {
        static string GetMimeType(FileResult fileResult)
        {
            if (!string.IsNullOrWhiteSpace(fileResult.ContentType))
                return fileResult.ContentType;

            var ext = Path.GetExtension(fileResult.FileName ?? fileResult.FullPath)?.ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        var endpoint = RestApiEndpoints.CreateCollectionItem;

        var files = new List<(string Name, string FileName, string ContentType, Stream Content)>();

        if (request.ObverseImage is not null)
        {
            var stream = await request.ObverseImage.OpenReadAsync();
            files.Add(
                (
                    "ObverseImage",
                    Path.GetFileName(request.ObverseImage.FileName ?? request.ObverseImage.FullPath),
                    GetMimeType(request.ObverseImage),
                    stream
                )
            );
        }

        if (request.ReverseImage is not null)
        {
            var stream = await request.ReverseImage.OpenReadAsync();
            files.Add(
                (
                    "ReverseImage",
                    Path.GetFileName(request.ReverseImage.FileName ?? request.ReverseImage.FullPath),
                    GetMimeType(request.ReverseImage),
                    stream
                )
            );
        }

        var requestBody = new CollectionItemCreationDto.Request
        {
            TypeId = request.TypeId,
            CountryId = request.CountryId,
            CollectionId = request.CollectionId,
            CollectionStatusId = request.CollectionStatusId,
            Value = request.Value,
            Currency = request.Currency,
            AdditionalInfo = request.AdditionalInfo,
            SerialNumber = request.SerialNumber,
            Description = request.Description,
            SpecialStatusId = request.SpecialStatusId,
            QualityId = request.QualityId,
        };

        var response = await _restApiService.SendMultipartRestApiRequest(endpoint, requestBody, files);

        return response?.Id;
    }

    public async Task<bool> DeleteCollectionItemAsync(Guid collectionId, int itemId)
    {
        var endpoint = RestApiEndpoints.DeleteCollectionItem(collectionId, itemId);

        return await _restApiService.SendRestApiRequest(endpoint);
    }
}
