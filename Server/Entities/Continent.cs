using Microsoft.EntityFrameworkCore;

namespace Server.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Continent
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
