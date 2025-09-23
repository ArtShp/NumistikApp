namespace Server.Entities;

public class Collection
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<UserCollection> UserCollections { get; set; } = [];

    public ICollection<CollectionItem> CollectionItems { get; set; } = [];
}
