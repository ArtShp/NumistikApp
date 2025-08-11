using Server.Entities;
using Server.Models.Extensions;

namespace Server.Models.CatalogItem;

public static class CatalogItemCreationDto
{
    public class Request
    {
        public required CatalogItemType Type { get; set; }

        [CatalogItemValue]
        public required string Value { get; set; }

        public required int CountryId { get; set; }

        public required bool IsMinor { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }

    public class Response
    {
        public required int Id { get; set; }
    }
}
