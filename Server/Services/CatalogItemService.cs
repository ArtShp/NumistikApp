using Server.Data;
using Server.Entities;
using Server.Models.CatalogItem;

namespace Server.Services;

public class CatalogItemService(MyDbContext context)
{
    public IQueryable<CatalogItemDto.Response> GetCatalogItems()
    {
        return context.CatalogItems.Select(ci =>
            new CatalogItemDto.Response
            {
                Id = ci.Id,
                Type = ci.Type,
                Value = ci.Value,
                CountryId = ci.CountryId,
                IsMinor = ci.IsMinor,
                Name = ci.Name,
                Description = ci.Description
            }
        );
    }

    public async Task<CatalogItemDto.Response?> GetCatalogItemAsync(int catalogItemId)
    {
        var catalogItem = await context.CatalogItems
            .FindAsync(catalogItemId);

        if (catalogItem is null) return null;

        return new CatalogItemDto.Response
        {
            Id = catalogItem.Id,
            Type = catalogItem.Type,
            Value = catalogItem.Value,
            CountryId = catalogItem.CountryId,
            IsMinor = catalogItem.IsMinor,
            Name = catalogItem.Name,
            Description = catalogItem.Description
        };
    }

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
