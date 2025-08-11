using Server.Data;
using Server.Entities;
using Server.Models.CollectionItem;

namespace Server.Services;

public class CollectionItemService(MyDbContext context)
{
    public IQueryable<CollectionItemDto.Response> GetCollectionItems(Guid userId, UserAppRole userAppRole)
    {
        IQueryable<CollectionItem> query = context.CollectionItems;

        if (userAppRole < UserAppRole.Admin)
        {
            query = query.Where(ci => ci.Collection.UserCollections.Any(c => c.UserId == userId));
        }

        return query.Select(ci => new CollectionItemDto.Response
        {
            Id = ci.Id,
            CatalogItemId = ci.CatalogItemId,
            CollectionStatus = ci.CollectionStatus,
            SpecialStatusId = ci.SpecialStatusId,
            Quality = ci.Quality,
            CollectionId = ci.CollectionId,
            SerialNumber = ci.SerialNumber,
            Description = ci.Description,
            ObverseImageUrl = GetImageAsync(ci.ObverseImageUrl).Result,
            ReverseImageUrl = GetImageAsync(ci.ReverseImageUrl).Result
        });
    }

    public async Task<CollectionItemDto.Response?> GetCollectionItemAsync(Guid userId, UserAppRole userAppRole, int collectionItemId)
    {
        var collectionItem = await context.CollectionItems
            .FindAsync(collectionItemId);

        if (collectionItem is null) return null;

        if (userAppRole < UserAppRole.Admin)
        {
            var userCollection = collectionItem.Collection.UserCollections.FirstOrDefault(c => c.UserId == userId);

            if (userCollection is null) return null;
        }

        return new CollectionItemDto.Response
        {
            Id = collectionItem.Id,
            CatalogItemId = collectionItem.CatalogItemId,
            CollectionStatus = collectionItem.CollectionStatus,
            SpecialStatusId = collectionItem.SpecialStatusId,
            Quality = collectionItem.Quality,
            CollectionId = collectionItem.CollectionId,
            SerialNumber = collectionItem.SerialNumber,
            Description = collectionItem.Description,
            ObverseImageUrl = await GetImageAsync(collectionItem.ObverseImageUrl),
            ReverseImageUrl = await GetImageAsync(collectionItem.ReverseImageUrl)
        };
    }

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

    private static async Task<IFormFile?> GetImageAsync(string? imageUrl)
    {
        if (imageUrl is null) return null;

        var filepath = Path.Combine("wwwroot", imageUrl.TrimStart('/'));

        if (!File.Exists(filepath)) return null;

        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);

        return new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filepath));
    }
}
