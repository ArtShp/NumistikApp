using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.CollectionItemType;

namespace Server.Services;

public class CollectionItemTypeService(MyDbContext context)
{
    public IQueryable<CollectionItemTypeDto.Response> GetCollectionItemTypes()
    {
        return context.CollectionItemTypes.Select(ci =>
            new CollectionItemTypeDto.Response
            {
                Id = ci.Id,
                Name = ci.Name
            }
        );
    }

    public async Task<CollectionItemTypeDto.Response?> GetCollectionItemTypeAsync(int typeId)
    {
        var type = await context.CollectionItemTypes
            .FindAsync(typeId);

        if (type is null) return null;

        return new CollectionItemTypeDto.Response
        {
            Id = type.Id,
            Name = type.Name
        };
    }

    public async Task<CollectionItemTypeCreationDto.Response?> CreateCollectionItemTypeAsync(CollectionItemTypeCreationDto.Request request)
    {
        var foundType = await context.CollectionItemTypes.FirstOrDefaultAsync(c => c.Name == request.Name);

        if (foundType is not null) return null;

        var type = new CollectionItemType
        {
            Name = request.Name
        };

        context.CollectionItemTypes.Add(type);
        await context.SaveChangesAsync();

        return new CollectionItemTypeCreationDto.Response
        {
            Id = type.Id,
            Name = type.Name
        };
    }

    public async Task<bool> UpdateCollectionItemTypeAsync(CollectionItemTypeUpdateDto.Request request)
    {
        var foundType = await context.CollectionItemTypes.FindAsync(request.Id);

        if (foundType is null) return false;

        foundType.Name = request.Name;

        context.CollectionItemTypes.Update(foundType);
        await context.SaveChangesAsync();

        return true;
    }
}
