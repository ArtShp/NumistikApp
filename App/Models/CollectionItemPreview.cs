namespace App.Models;

public class CollectionItemPreview
{
    public required int Id { get; set; }
    public required Guid CollectionId { get; set; }
    public required string Value { get; set; }
    public required string Currency { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public string? ObverseImageUrl { get; set; }
    public string? ReverseImageUrl { get; set; }

    public string? TypeName { get; set; }
    public string? CountryName { get; set; }
    public string? StatusName { get; set; }
    public string? QualityName { get; set; }
    public string? SpecialStatusName { get; set; }

    public string ValueCurrencyInfo
    {
        get
        {
            var main = Value + ' ' + Currency;
            if (!string.IsNullOrWhiteSpace(AdditionalInfo))
            {
                return $"{main} ({AdditionalInfo})";
            }

            return main;
        }
    }

    public string? SecondaryInfo
    {
        get
        {
            var parts = new List<string>(5);
            if (!string.IsNullOrWhiteSpace(TypeName)) parts.Add(TypeName!);
            if (!string.IsNullOrWhiteSpace(CountryName)) parts.Add(CountryName!);
            if (!string.IsNullOrWhiteSpace(StatusName)) parts.Add(StatusName!);
            if (!string.IsNullOrWhiteSpace(QualityName)) parts.Add(QualityName!);
            if (!string.IsNullOrWhiteSpace(SpecialStatusName)) parts.Add(SpecialStatusName!);

            return parts.Count == 0 ? null : string.Join(" · ", parts);
        }
    }
}
