using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Shared.Models.CollectionItemStatus;

namespace Server.Services;

public class CollectionItemStatusService(MyDbContext context)
{
    public async Task<List<CollectionItemStatusDto.Response>> GetCollectionItemStatusesAsync(int? lastSeenId, int pageSize)
    {
        IQueryable<CollectionItemStatus> query = context.CollectionItemStatuses
            .OrderBy(ciq => ciq.Id);

        if (lastSeenId.HasValue)
        {
            query = query.Where(ciq => ciq.Id > lastSeenId.Value);
        }

        return await query.Take(pageSize).Select(ci =>
            new CollectionItemStatusDto.Response
            {
                Id = ci.Id,
                Name = ci.Name
            })
            .ToListAsync();
    }

    public async Task<CollectionItemStatusDto.Response?> GetCollectionItemStatusAsync(int statusId)
    {
        var status = await context.CollectionItemStatuses
            .FindAsync(statusId);

        if (status is null) return null;

        return new CollectionItemStatusDto.Response
        {
            Id = status.Id,
            Name = status.Name
        };
    }

    public async Task<CollectionItemStatusCreationDto.Response?> CreateCollectionItemStatusAsync(CollectionItemStatusCreationDto.Request request)
    {
        var foundStatus = await context.CollectionItemStatuses.FirstOrDefaultAsync(c => c.Name == request.Name);

        if (foundStatus is not null) return null;

        var status = new CollectionItemStatus
        {
            Name = request.Name
        };

        context.CollectionItemStatuses.Add(status);
        await context.SaveChangesAsync();

        return new CollectionItemStatusCreationDto.Response
        {
            Id = status.Id,
            Name = status.Name
        };
    }

    public async Task<bool> UpdateCollectionItemStatusAsync(CollectionItemStatusUpdateDto.Request request)
    {
        var foundStatus = await context.CollectionItemStatuses.FindAsync(request.Id);

        if (foundStatus is null) return false;

        foundStatus.Name = request.Name;

        context.CollectionItemStatuses.Update(foundStatus);
        await context.SaveChangesAsync();

        return true;
    }
}
