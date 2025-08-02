using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

public class UserCollection
{
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    [ForeignKey(nameof(Collection))]
    public Guid CollectionId { get; set; }

    public Collection Collection { get; set; } = null!;

    public CollectionRole Role { get; set; }
}

public enum CollectionRole
{
    Viewer = 1,
    Editor = 2,
    Admin = 3,
    Owner = 4
}
