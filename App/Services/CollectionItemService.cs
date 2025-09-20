using App.Helpers;
using App.Models;
using Shared.Models.CollectionItem;

namespace App.Services;

internal class CollectionItemService(IRestApiService restApiService, ILookupService lookupService) : ICollectionItemService
{
    private readonly IRestApiService _restApiService = restApiService;
    private readonly ILookupService _lookupService = lookupService;

    public async Task<IEnumerable<CollectionItem>> GetCollectionItemsAsync(Guid collectionId, int? lastSeenId)
    {
        var endpoint = RestApiEndpoints.GetCollectionItems(collectionId);
        var query = new Dictionary<string, string?> { ["lastSeenId"] = lastSeenId?.ToString() };

        var result = await _restApiService.SendRestApiRequest(endpoint, query);

        if (result is null) return [];

        var previews = new List<CollectionItem>(result.Count);

        foreach (var item in result)
        {
            var typeTask = _lookupService.GetTypeNameAsync(item.TypeId);
            var countryTask = _lookupService.GetCountryNameAsync(item.CountryId);
            var statusTask = _lookupService.GetStatusNameAsync(item.CollectionStatusId);
            var qualityTask = item.QualityId.HasValue ? _lookupService.GetQualityNameAsync(item.QualityId.Value) : Task.FromResult<string?>(null);
            var specialStatusTask = item.SpecialStatusId.HasValue ? _lookupService.GetSpecialStatusNameAsync(item.SpecialStatusId.Value) : Task.FromResult<string?>(null);

            await Task.WhenAll(typeTask, countryTask, statusTask, qualityTask, specialStatusTask);

            var preview = new CollectionItem
            {
                Id = item.Id,
                CollectionId = item.CollectionId,
                Value = item.Value,
                Currency = item.Currency,
                AdditionalInfo = item.AdditionalInfo,
                SerialNumber = item.SerialNumber,
                Description = item.Description,
                ObverseImageUrl = item.ObverseImageUrl,
                ReverseImageUrl = item.ReverseImageUrl,
                TypeName = typeTask.Result,
                CountryName = countryTask.Result,
                StatusName = statusTask.Result,
                QualityName = qualityTask.Result,
                SpecialStatusName = specialStatusTask.Result
            };

            previews.Add(preview);
        }

        return previews;
    }

    public async Task<int?> CreateCollectionItemAsync(CollectionItemCreateRequest request)
    {
        var endpoint = RestApiEndpoints.CreateCollectionItem;

        var files = new List<(string Name, string FileName, string ContentType, Stream Content)>();

        if (request.ObverseImage is not null)
        {
            var stream = await request.ObverseImage.OpenReadAsync();
            files.Add(
                (
                    "ObverseImage",
                    Path.GetFileName(request.ObverseImage.FileName),
                    FileMimeHelper.GetContentType(request.ObverseImage),
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
                    Path.GetFileName(request.ReverseImage.FileName),
                    FileMimeHelper.GetContentType(request.ReverseImage),
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

    public async Task<bool> UpdateCollectionItemAsync(CollectionItemUpdateRequest request)
    {
        var endpoint = RestApiEndpoints.UpdateCollectionItem;

        var files = new List<(string Name, string FileName, string ContentType, Stream Content)>();

        if (request.ObverseImage is not null)
        {
            var stream = await request.ObverseImage.OpenReadAsync();
            files.Add(
                (
                    "ObverseImage",
                    Path.GetFileName(request.ObverseImage.FileName),
                    FileMimeHelper.GetContentType(request.ObverseImage),
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
                    Path.GetFileName(request.ReverseImage.FileName),
                    FileMimeHelper.GetContentType(request.ReverseImage),
                    stream
                )
            );
        }

        var requestBody = new CollectionItemUpdateDto.Request
        {
            Id = request.Id,
            CollectionId = request.CollectionId,
            TypeId = request.TypeId,
            CountryId = request.CountryId,
            CollectionStatusId = request.CollectionStatusId,
            SpecialStatusId = request.SpecialStatusId,
            QualityId = request.QualityId,
            Value = request.Value,
            Currency = request.Currency,
            AdditionalInfo = request.AdditionalInfo,
            SerialNumber = request.SerialNumber,
            Description = request.Description
        };

        var response = await _restApiService.SendMultipartRestApiRequest(endpoint, requestBody, files);
        return response;
    }

    public async Task<bool> DeleteCollectionItemAsync(Guid collectionId, int itemId)
    {
        var endpoint = RestApiEndpoints.DeleteCollectionItem(collectionId, itemId);

        return await _restApiService.SendRestApiRequest(endpoint);
    }
}
