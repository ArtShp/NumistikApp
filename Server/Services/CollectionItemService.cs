using Server.Data;
using Server.Entities;
using Server.Models;

namespace Server.Services;

public class CollectionItemService(MyDbContext context)
{
    public async Task<CollectionItemCreationDto.Response?> CreateCollectionItemAsync(CollectionItemCreationDto.Request request)
    {
        var catalogItem = await context.CatalogItems
            .FindAsync(request.CatalogItemId);
        if (catalogItem is null) return null;


        var specialStatus = await context.CollectionItemSpecialStatuses
            .FindAsync(request.SpecialStatusId);
        if (specialStatus is null && request.SpecialStatusId.HasValue)
            return null;

        var collection = await context.Collections
            .FindAsync(request.CollectionId);
        if (collection is null) return null;

        var collectionItem = new CollectionItem
        {
            CatalogItem = catalogItem,
            CollectionStatus = request.CollectionStatus,
            SpecialStatus = specialStatus,
            Quality = request.Quality,
            Collection = collection,
            SerialNumber = request.SerialNumber,
            Description = request.Description,
            ObverseImageUrl = await SaveImageAsync(request.ObverseImage),
            ReverseImageUrl = await SaveImageAsync(request.ReverseImage)
        };

        context.CollectionItems.Add(collectionItem);
        await context.SaveChangesAsync();

        return new CollectionItemCreationDto.Response
        {
            Id = collectionItem.Id
        };
    }

    public async Task<bool> UpdateCollectionItemAsync(CollectionItemUpdateDto.Request request)
    {
        var foundCollectionItem = await context.CollectionItems.FindAsync(request.Id);

        if (foundCollectionItem is null) return false;

        if (request.CatalogItemId.HasValue)
        {
            var catalogItem = await context.CatalogItems
                .FindAsync(request.CatalogItemId.Value);
            if (catalogItem is null) return false;

            foundCollectionItem.CatalogItem = catalogItem;
        }
        if (request.CollectionStatus.HasValue)
        {
            foundCollectionItem.CollectionStatus = request.CollectionStatus.Value;
        }
        if (request.SpecialStatusId.HasValue)
        {
            var specialStatus = await context.CollectionItemSpecialStatuses
                .FindAsync(request.SpecialStatusId.Value);
            if (specialStatus is null) return false;

            foundCollectionItem.SpecialStatus = specialStatus;
        }
        if (request.Quality.HasValue)
        {
            foundCollectionItem.Quality = request.Quality.Value;
        }
        if (request.CollectionId.HasValue)
        {
            var collection = await context.Collections
                .FindAsync(request.CollectionId.Value);
            if (collection is null) return false;

            foundCollectionItem.Collection = collection;
        }
        if (request.SerialNumber is not null)
        {
            foundCollectionItem.SerialNumber = request.SerialNumber;
        }
        if (request.Description is not null)
        {
            foundCollectionItem.Description = request.Description;
        }
        if (request.ObverseImage is not null)
        {
            foundCollectionItem.ObverseImageUrl = await SaveImageAsync(request.ObverseImage);
        }
        if (request.ReverseImage is not null)
        {
            foundCollectionItem.ReverseImageUrl = await SaveImageAsync(request.ReverseImage);
        }

        context.CollectionItems.Update(foundCollectionItem);
        await context.SaveChangesAsync();

        return true;
    }

    private static async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file is null) return null;

        var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var uploadsDir = Path.Combine("wwwroot", "uploads");
        Directory.CreateDirectory(uploadsDir);

        var filepath = Path.Combine(uploadsDir, filename);

        using (var stream = new FileStream(filepath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{filename}";
    }
}
