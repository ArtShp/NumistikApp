using Server.Data;
using Server.Entities;
using Server.Models;

namespace Server.Services;

public class CatalogItemService(MyDbContext context)
{
    public async Task<CatalogItemCreationDto.Response?> CreateCatalogItemAsync(CatalogItemCreationDto.Request request)
    {
        var country = await context.Countries.FindAsync(request.CountryId);

        if (country is null) return null;

        var catalogItem = new CatalogItem
        {
            Type = request.Type,
            Value = request.Value,
            Country = country,
            IsMinor = request.IsMinor,
            Name = request.Name,
            Description = request.Description
        };

        context.CatalogItems.Add(catalogItem);
        await context.SaveChangesAsync();

        return new CatalogItemCreationDto.Response
        {
            Id = catalogItem.Id
        };
    }

    public async Task<bool> UpdateCatalogItemAsync(CatalogItemUpdateDto.Request request)
    {
        var foundCatalogItem = await context.CatalogItems.FindAsync(request.Id);

        if (foundCatalogItem is null) return false;

        if (request.Type is not null) foundCatalogItem.Type = request.Type.Value;
        if (request.Value is not null) foundCatalogItem.Value = request.Value;
        if (request.CountryId.HasValue)
        {
            var country = await context.Countries.FindAsync(request.CountryId.Value);
            if (country is null) return false;

            foundCatalogItem.Country = country;
        }
        if (request.IsMinor.HasValue) foundCatalogItem.IsMinor = request.IsMinor.Value;
        if (request.Name is not null) foundCatalogItem.Name = request.Name;
        if (request.Description is not null) foundCatalogItem.Description = request.Description;

        context.CatalogItems.Update(foundCatalogItem);
        await context.SaveChangesAsync();

        return true;
    }
}
