namespace App.Models;

public class MyCollectionDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    public required string CollectionRole { get; set; }
}
