using Microsoft.EntityFrameworkCore;

namespace Server.Entities;

[Index(nameof(Code), IsUnique = true)]
public class Currency
{
    public int Id { get; set; }

    public string MajorName { get; set; } = string.Empty;

    public string MinorName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;
}
