using Server.Entities;

namespace Server.Models.CollectionItem;

public static class CollectionItemDto
{
    public class Response
    {
        public required int Id { get; set; }

        public required CollectionItemType Type { get; set; }

        public required int CountryId { get; set; }

        public required CollectionItemStatus CollectionStatus { get; set; }

        public required int? SpecialStatusId { get; set; }

        public required CollectionItemQuality? Quality { get; set; }

        public required Guid CollectionId { get; set; }

        public required string Value { get; set; }

        public required string Currency { get; set; }

        public required string? AdditionalInfo { get; set; }

        public required string? SerialNumber { get; set; }

        public required string? Description { get; set; }

        public required string? ObverseImageUrl { get; set; }

        public required string? ReverseImageUrl { get; set; }
    }
}
