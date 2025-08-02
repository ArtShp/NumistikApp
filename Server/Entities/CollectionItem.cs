using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

public class CollectionItem
{
    public int Id { get; set; }

    [ForeignKey(nameof(CatalogItem))]
    public int CatalogItemId { get; set; }

    public CatalogItem CatalogItem { get; set; } = null!;

    public CollectionItemStatus CollectionStatus { get; set; }

    [ForeignKey(nameof(SpecialStatus))]
    public int? SpecialStatusId { get; set; }

    public CollectionItemSpecialStatus? SpecialStatus { get; set; }

    public CollectionItemQuality? Quality { get; set; }

    [ForeignKey(nameof(Collection))]
    public Guid CollectionId { get; set; }

    public Collection Collection { get; set; } = null!;

    public string? SerialNumber { get; set; }

    public string? Description { get; set; }

    public string? ObverseImageUrl { get; set; }

    public string? ReverseImageUrl { get; set; }
}

[Index(nameof(Name), IsUnique = true)]
public class CollectionItemSpecialStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public enum CollectionItemStatus
{
    InCollection = 1,
    ForSaleOrExchange = 2,
    SoldOrExchanged = 3,
    Lost = 4,
}

public enum CollectionItemQuality
{
    Uncirculated = 1,
    ExtremelyFine = 2,
    VeryFine = 3,
    Fine = 4,
    VeryGood = 5,
    Good = 6,
    Poor = 7
}
