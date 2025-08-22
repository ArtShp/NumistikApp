using Server.Entities;
using Server.Models.Extensions;

namespace Server.Models.CollectionItem;

public static class CollectionItemUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        public int? TypeId { get; set; }

        public int? CountryId { get; set; }

        public CollectionItemStatus? CollectionStatus { get; set; }

        public int? SpecialStatusId { get; set; }

        public CollectionItemQuality? Quality { get; set; }

        public Guid? CollectionId { get; set; }

        [CatalogItemValue]
        public string? Value { get; set; }

        public string? Currency { get; set; }

        public string? AdditionalInfo { get; set; }

        public string? SerialNumber { get; set; }

        public string? Description { get; set; }

        [AllowedImageFormat]
        public IFormFile? ObverseImage { get; set; }

        [AllowedImageFormat]
        public IFormFile? ReverseImage { get; set; }
    }
}
