using Server.Data;
using Server.Entities;
using Server.Models.CollectionItem;

namespace Server.Services;

public class CollectionItemService(MyDbContext context, IWebHostEnvironment env)
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
            TypeId = ci.TypeId,
            CountryId = ci.CountryId,
            CollectionStatusId = ci.CollectionStatusId,
            SpecialStatusId = ci.SpecialStatusId,
            QualityId = ci.QualityId,
            CollectionId = ci.CollectionId,
            Value = ci.Value,
            Currency = ci.Currency,
            AdditionalInfo = ci.AdditionalInfo,
            SerialNumber = ci.SerialNumber,
            Description = ci.Description,
            ObverseImageUrl = ci.ObverseImageUrl,
            ReverseImageUrl = ci.ReverseImageUrl
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
            TypeId = collectionItem.TypeId,
            CountryId = collectionItem.CountryId,
            CollectionStatusId = collectionItem.CollectionStatusId,
            SpecialStatusId = collectionItem.SpecialStatusId,
            QualityId = collectionItem.QualityId,
            CollectionId = collectionItem.CollectionId,
            Value = collectionItem.Value,
            Currency = collectionItem.Currency,
            AdditionalInfo = collectionItem.AdditionalInfo,
            SerialNumber = collectionItem.SerialNumber,
            Description = collectionItem.Description,
            ObverseImageUrl = collectionItem.ObverseImageUrl,
            ReverseImageUrl = collectionItem.ReverseImageUrl
        };
    }

    public async Task<CollectionItemCreationDto.Response?> CreateCollectionItemAsync(Guid userId, CollectionItemCreationDto.Request request)
    {
        var type = await context.CollectionItemTypes
            .FindAsync(request.TypeId);
        if (type is null) return null;

        var country = await context.Countries
            .FindAsync(request.CountryId);
        if (country is null) return null;

        var status = await context.CollectionItemStatuses
            .FindAsync(request.CollectionStatusId);
        if (status is null) return null;

        var specialStatus = await context.CollectionItemSpecialStatuses
            .FindAsync(request.SpecialStatusId);
        if (specialStatus is null && request.SpecialStatusId.HasValue)
            return null;

        var quality = await context.CollectionItemQualities
            .FindAsync(request.QualityId);
        if (quality is null && request.QualityId.HasValue)
            return null;

        var collection = await context.Collections
            .FindAsync(request.CollectionId);
        if (collection is null) return null;

        var userCollection = collection.UserCollections
            .FirstOrDefault(uc => uc.UserId == userId);
        if (userCollection is null || userCollection.Role < CollectionRole.Editor) return null;

        var collectionItem = new CollectionItem
        {
            Type = type,
            Country = country,
            CollectionStatus = status,
            SpecialStatus = specialStatus,
            Quality = quality,
            Collection = collection,
            Value = request.Value,
            Currency = request.Currency,
            AdditionalInfo = request.AdditionalInfo,
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

    public async Task<bool> UpdateCollectionItemAsync(Guid userId, CollectionItemUpdateDto.Request request)
    {
        var foundCollectionItem = await context.CollectionItems.FindAsync(request.Id);
        if (foundCollectionItem is null) return false;

        var userCollection = foundCollectionItem.Collection.UserCollections
            .FirstOrDefault(uc => uc.UserId == userId);
        if (userCollection is null || userCollection.Role < CollectionRole.Editor) return false;

        if (request.TypeId.HasValue)
        {
            var type = await context.CollectionItemTypes
                .FindAsync(request.TypeId.Value);
            if (type is null) return false;

            foundCollectionItem.Type = type;
        }
        if (request.CountryId.HasValue)
        {
            var country = await context.Countries
                .FindAsync(request.CountryId.Value);
            if (country is null) return false;

            foundCollectionItem.Country = country;
        }
        if (request.CollectionStatusId.HasValue)
        {
            var status = await context.CollectionItemStatuses
                .FindAsync(request.CollectionStatusId.Value);
            if (status is null) return false;

            foundCollectionItem.CollectionStatus = status;
        }
        if (request.SpecialStatusId.HasValue)
        {
            var specialStatus = await context.CollectionItemSpecialStatuses
                .FindAsync(request.SpecialStatusId.Value);
            if (specialStatus is null) return false;

            foundCollectionItem.SpecialStatus = specialStatus;
        }
        if (request.QualityId.HasValue)
        {
            var quality = await context.CollectionItemQualities
                .FindAsync(request.QualityId.Value);
            if (quality is null) return false;

            foundCollectionItem.Quality = quality;
        }
        if (request.CollectionId.HasValue)
        {
            var collection = await context.Collections
                .FindAsync(request.CollectionId.Value);
            if (collection is null) return false;

            foundCollectionItem.Collection = collection;
        }
        if (request.Value is not null)
        {
            foundCollectionItem.Value = request.Value;
        }
        if (request.Currency is not null)
        {
            foundCollectionItem.Currency = request.Currency;
        }
        if (request.AdditionalInfo is not null)
        {
            foundCollectionItem.AdditionalInfo = request.AdditionalInfo;
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

    public string? GetImagePath(string? filename)
    {
        if (filename is null) return null;

        var filepath = Path.Combine(env.ContentRootPath, "static", "images", filename);

        if (!File.Exists(filepath)) return null;

        return filepath;
    }

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file is null) return null;

        var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var uploadsDir = Path.Combine(env.ContentRootPath, "static", "images");
        Directory.CreateDirectory(uploadsDir);

        var filepath = Path.Combine(uploadsDir, filename);

        using (var stream = new FileStream(filepath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filename;
    }
}
