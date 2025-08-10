using Microsoft.EntityFrameworkCore;

namespace Server.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Continent : IHasIntId
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
