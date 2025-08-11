using Server.Entities;
using Server.Models.Extensions;

namespace Server.Models.CollectionItem;

public static class CollectionItemCreationDto
{
    public class Request
    {
        public required int CatalogItemId { get; set; }

        public required CollectionItemStatus CollectionStatus { get; set; }

        public int? SpecialStatusId { get; set; }

        public CollectionItemQuality? Quality { get; set; }

        public required Guid CollectionId { get; set; }

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
