using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.Collection;

namespace Server.Services;

public class CollectionService(MyDbContext context)
{
    public IQueryable<CollectionDto.Response> GetAllCollections(Guid userId)
    {
        return context.UserCollections
            .Where(uc => uc.UserId == userId)
            .Select(uc => new CollectionDto.Response
            {
                Id = uc.Collection.Id,
                Name = uc.Collection.Name,
                Description = uc.Collection.Description,
                CollectionRole = uc.Role
            });
    }

    public async Task<CollectionDto.Response?> GetCollectionAsync(Guid userId, Guid collectionId)
    {
        var userCollection = await context.UserCollections
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CollectionId == collectionId);

        if (userCollection is null) return null;

        var collection = await context.Collections
            .FindAsync(collectionId);

        if (collection is null) return null;

        return new CollectionDto.Response
        {
            Id = collection.Id,
            Name = collection.Name,
            Description = collection.Description,
            CollectionRole = userCollection.Role
        };
    }

    public async Task<CollectionCreationDto.Response?> CreateCollectionAsync(Guid userId, CollectionCreationDto.Request request)
    {
        var collection = new Collection
        {
            Name = request.Name,
            Description = request.Description
        };

        context.Collections.Add(collection);
        await context.SaveChangesAsync();

        var userCollection = new UserCollection
        {
            UserId = userId,
            CollectionId = collection.Id,
            Role = CollectionRole.Owner
        };

        context.UserCollections.Add(userCollection);
        await context.SaveChangesAsync();

        return new CollectionCreationDto.Response
        {
            Id = collection.Id
        };
    }

    public async Task<bool> AssignCollectionRoleAsync(Guid userId, CollectionAssignDto.Request request)
    {
        var userCollection = await context.UserCollections
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CollectionId == request.CollectionId);

        if (userCollection is null) return false;

        if (userCollection.Role <= request.Role || userCollection.Role < CollectionRole.Admin)
            return false;

        var targetUserCollection = await context.UserCollections
            .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.CollectionId == request.CollectionId);

        if (targetUserCollection is null)
        {
            if (await context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId) is null)
                return false;

            // assign
            targetUserCollection = new UserCollection
            {
                UserId = request.UserId,
                CollectionId = request.CollectionId,
                Role = request.Role
            };
            context.UserCollections.Add(targetUserCollection);

            await context.SaveChangesAsync();
        }
        else if (targetUserCollection.Role != request.Role)
        {
            // update
            targetUserCollection.Role = request.Role;
            context.UserCollections.Update(targetUserCollection);

            await context.SaveChangesAsync();
        }
        // no update needed

        return true;
    }
}
