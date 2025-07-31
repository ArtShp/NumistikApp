namespace Server.Entities;

public class Collection
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<UserCollection> UserCollections { get; set; } = [];
}
