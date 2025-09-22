using Microsoft.AspNetCore.Http;
using Shared.Models.Extensions;

namespace Shared.Models.CollectionItem;

public static class CollectionItemCreationDto
{
    public class Request
    {
        public required int TypeId { get; set; }

        public required int CountryId { get; set; }

        public required int CollectionStatusId { get; set; }

        public int? SpecialStatusId { get; set; }

        public int? QualityId { get; set; }

        public required Guid CollectionId { get; set; }

        [CatalogItemValue]
        public required string Value { get; set; }

        public required string Currency { get; set; }

        public string? AdditionalInfo { get; set; }

        public string? SerialNumber { get; set; }

        public string? Description { get; set; }

        [AllowedImageFormat]
        public IFormFile? ObverseImage { get; set; }

        [AllowedImageFormat]
        public IFormFile? ReverseImage { get; set; }
    }

    public class Response
    {
        public required int Id { get; set; }
    }
}
