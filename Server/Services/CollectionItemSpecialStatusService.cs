using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Shared.Models.CollectionItemSpecialStatus;

namespace Server.Services;

public class CollectionItemSpecialStatusService(MyDbContext context)
{
    public async Task<List<CollectionItemSpecialStatusDto.Response>> GetCollectionItemSpecialStatusesAsync(int? lastSeenId, int pageSize)
    {
        IQueryable<CollectionItemSpecialStatus> query = context.CollectionItemSpecialStatuses
            .OrderBy(ciq => ciq.Id);

        if (lastSeenId.HasValue)
        {
            query = query.Where(ciq => ciq.Id > lastSeenId.Value);
        }

        return await query.Take(pageSize).Select(ci =>
            new CollectionItemSpecialStatusDto.Response
            {
                Id = ci.Id,
                Name = ci.Name
            })
            .ToListAsync();
    }

    public async Task<CollectionItemSpecialStatusDto.Response?> GetCollectionItemSpecialStatusAsync(int specialStatusId)
    {
        var specialStatus = await context.CollectionItemSpecialStatuses
            .FindAsync(specialStatusId);

        if (specialStatus is null) return null;

        return new CollectionItemSpecialStatusDto.Response
        {
            Id = specialStatus.Id,
            Name = specialStatus.Name
        };
    }

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
