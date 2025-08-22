using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

public class CollectionItem
{
    public int Id { get; set; }

    [ForeignKey(nameof(Type))]
    public int TypeId { get; set; }

    public CollectionItemType Type { get; set; } = null!;

    [ForeignKey(nameof(Country))]
    public int CountryId { get; set; }

    public Country Country { get; set; } = null!;

    public CollectionItemStatus CollectionStatus { get; set; }

    [ForeignKey(nameof(SpecialStatus))]
    public int? SpecialStatusId { get; set; }

    public CollectionItemSpecialStatus? SpecialStatus { get; set; }

    public CollectionItemQuality? Quality { get; set; }

    [ForeignKey(nameof(Collection))]
    public Guid CollectionId { get; set; }

    public Collection Collection { get; set; } = null!;

    public string Value { get; set; } = string.Empty;

    public string Currency { get; set; } = string.Empty;

    public string? AdditionalInfo { get; set; }

    public string? SerialNumber { get; set; }

    public string? Description { get; set; }

    public string? ObverseImageUrl { get; set; }

    public string? ReverseImageUrl { get; set; }
}

[Index(nameof(Name), IsUnique = true)]
public class CollectionItemSpecialStatus : IHasIntId
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

[Index(nameof(Name), IsUnique = true)]
public class CollectionItemType : IHasIntId
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
