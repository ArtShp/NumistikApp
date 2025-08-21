using Server.Entities;

namespace Server.Models.CatalogItem;

public static class CatalogItemDto
{
    public class Response
    {
        public required int Id { get; set; }

        public required CatalogItemType Type { get; set; }

        public required string Value { get; set; }

        public required int CountryId { get; set; }

        public required bool IsMinor { get; set; }

        public required string? Name { get; set; }

        public required string? Description { get; set; }
    }
}
