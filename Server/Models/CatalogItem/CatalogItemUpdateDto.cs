using Server.Entities;
using Server.Models.Extensions;

namespace Server.Models.CatalogItem;

public static class CatalogItemUpdateDto
{
    public class Request
    {
        public required int Id { get; set; }

        public CatalogItemType? Type { get; set; }

        [CatalogItemValue]
        public string? Value { get; set; }

        public int? CountryId { get; set; }

        public bool? IsMinor { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
