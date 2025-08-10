using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

public class CatalogItem
{
    public int Id { get; set; }

    public CatalogItemType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    [ForeignKey(nameof(Country))]
    public int CountryId { get; set; }

    public Country Country { get; set; } = null!;

    public bool IsMinor { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}

public enum CatalogItemType
{
    Banknote = 1,
    Coin = 2,
    Other = 3
}
