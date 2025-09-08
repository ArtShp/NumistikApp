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

    public string ValueCurrencyInfo
    {
        get
        {
            var main = Value + ' ' + Currency;
            if (!string.IsNullOrWhiteSpace(AdditionalInfo))
            {
                var info = AdditionalInfo;
                return $"{main} ({AdditionalInfo})";
            }

            return main;
        }
    }
}
