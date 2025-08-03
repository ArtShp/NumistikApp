using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Server.Data;

public static class DataSeeder
{
    public static void SeedAll(MyDbContext dbContext)
    {
        SeedItems(dbContext.Continents, "Continents");
        SeedItems(dbContext.CollectionItemSpecialStatuses, "CollectionItemSpecialStatuses");
        SeedItems(dbContext.Currencies, "Currencies");

        dbContext.SaveChanges();
    }

    private static void SeedItems<T>(DbSet<T> data, string filename) where T : class
    {
        if (data.Any()) return;

        using var reader = new StreamReader("Data/Seeds/" + filename + "_seed.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        data.AddRange(csv.GetRecords<T>());
    }
}
