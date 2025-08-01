using Server.Data;
using Server.Entities;
using Server.Models;

namespace Server.Services;

public class CollectionService(MyDbContext context)
{
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
}
