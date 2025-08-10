using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models;

namespace Server.Services;

public class CollectionItemSpecialStatusService(MyDbContext context)
{
    public async Task<CollectionItemSpecialStatusCreationDto.Response?> CreateCollectionItemSpecialStatusAsync(CollectionItemSpecialStatusCreationDto.Request request)
    {
        var foundSpecialStatus = await context.CollectionItemSpecialStatuses.FirstOrDefaultAsync(c => c.Name == request.Name);

        if (foundSpecialStatus is not null) return null;

        var specialStatus = new CollectionItemSpecialStatus
        {
            Name = request.Name
        };

        context.CollectionItemSpecialStatuses.Add(specialStatus);
        await context.SaveChangesAsync();

        return new CollectionItemSpecialStatusCreationDto.Response
        {
            Id = specialStatus.Id,
            Name = specialStatus.Name
        };
    }

    public async Task<bool> UpdateCollectionItemSpecialStatusAsync(CollectionItemSpecialStatusUpdateDto.Request request)
    {
        var foundSpecialStatus = await context.CollectionItemSpecialStatuses.FirstOrDefaultAsync(c => c.Id == request.Id);

        if (foundSpecialStatus is null) return false;

        foundSpecialStatus.Name = request.Name;

        context.CollectionItemSpecialStatuses.Update(foundSpecialStatus);
        await context.SaveChangesAsync();

        return true;
    }
}
