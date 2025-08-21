using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Code), IsUnique = true)]
public class Country : IHasIntId
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    [ForeignKey(nameof(Continent))]
    public int ContinentId { get; set; }

    [CsvHelper.Configuration.Attributes.Ignore]
    public Continent Continent { get; set; } = null!;
}
