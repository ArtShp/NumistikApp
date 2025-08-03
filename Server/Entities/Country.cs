using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    [ForeignKey(nameof(Continent))]
    public int ContinentId { get; set; }

    public Continent Continent { get; set; } = null!;

    [ForeignKey(nameof(Currency))]
    public int CurrencyId { get; set; }

    public Currency Currency { get; set; } = null!;
}
