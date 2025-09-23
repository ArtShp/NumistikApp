namespace App.Models;

public class CollectionItemCreateRequest
{
    public required int TypeId { get; set; }
    public required int CountryId { get; set; }
    public required int CollectionStatusId { get; set; }
    public int? SpecialStatusId { get; set; }
    public int? QualityId { get; set; }

    public required Guid CollectionId { get; set; }

    public required string Value { get; set; }
    public required string Currency { get; set; }

    public string? AdditionalInfo { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }

    public FileResult? ObverseImage { get; set; }
    public FileResult? ReverseImage { get; set; }
}
