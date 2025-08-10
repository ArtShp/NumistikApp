using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Server.Entities;
using System.Globalization;

namespace Server.Data;

public static class DataSeeder
{
    private static readonly CsvConfiguration csvConfiguration = new (CultureInfo.InvariantCulture)
    {
        MissingFieldFound = null,
        HeaderValidated = null,
    };

    public static void SeedAll(MyDbContext dbContext)
    {
        SeedItems(dbContext, dbContext.Continents, "Continents");
        SeedItems(dbContext, dbContext.CollectionItemSpecialStatuses, "CollectionItemSpecialStatuses");
        SeedItems(dbContext, dbContext.Currencies, "Currencies");
        SeedItems(dbContext, dbContext.Countries, "Countries");
    }

    private static void SeedItems<T>(DbContext context, DbSet<T> data, string filename) where T : class, IHasIntId
    {
        if (data.Any()) return;

        using var reader = new StreamReader("Data/Seeds/" + filename + "_seed.csv");
        using var csv = new CsvReader(reader, csvConfiguration);

        int maxId = 0;

        data.AddRange(csv.GetRecords<T>().Select(x => {
            ++maxId; return x;
        }));

        context.SaveChanges();

        context.Database.ExecuteSqlRaw(
            $"SELECT setval(pg_get_serial_sequence('\"{filename}\"', 'Id'), {maxId}, true)"
        );

        context.SaveChanges();
    }
}
