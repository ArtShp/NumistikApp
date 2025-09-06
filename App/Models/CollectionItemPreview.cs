namespace App.Models;

public class CollectionItemPreview
{
    public required int Id { get; set; }
    public required Guid CollectionId { get; set; }
    public string? Value { get; set; }
    public string? Currency { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public string? ObverseImageUrl { get; set; }
}
