namespace App.Models;

public sealed class LookupItem
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public override string ToString() => Name;
}
