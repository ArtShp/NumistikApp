using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.CollectionItemQuality;

namespace Server.Services;

public class CollectionItemQualityService(MyDbContext context)
{
    public async Task<List<CollectionItemQualityDto.Response>> GetCollectionItemQualitiesAsync(int? lastSeenId, int pageSize)
    {
        IQueryable<CollectionItemQuality> query = context.CollectionItemQualities
            .OrderBy(ciq => ciq.Id);

        if (lastSeenId.HasValue)
        {
            query = query.Where(ciq => ciq.Id > lastSeenId.Value);
        }

        return await query.Take(pageSize).Select(ci =>
            new CollectionItemQualityDto.Response
            {
                Id = ci.Id,
                Name = ci.Name
            })
            .ToListAsync();
    }

    public async Task<CollectionItemQualityDto.Response?> GetCollectionItemQualityAsync(int qualityId)
    {
        var quality = await context.CollectionItemQualities
            .FindAsync(qualityId);

        if (quality is null) return null;

        return new CollectionItemQualityDto.Response
        {
            Id = quality.Id,
            Name = quality.Name
        };
    }

    public async Task<CollectionItemQualityCreationDto.Response?> CreateCollectionItemQualityAsync(CollectionItemQualityCreationDto.Request request)
    {
        var foundQuality = await context.CollectionItemQualities.FirstOrDefaultAsync(c => c.Name == request.Name);

        if (foundQuality is not null) return null;

        var quality = new CollectionItemQuality
        {
            Name = request.Name
        };

        context.CollectionItemQualities.Add(quality);
        await context.SaveChangesAsync();

        return new CollectionItemQualityCreationDto.Response
        {
            Id = quality.Id,
            Name = quality.Name
        };
    }

    public async Task<bool> UpdateCollectionItemQualityAsync(CollectionItemQualityUpdateDto.Request request)
    {
        var foundQuality = await context.CollectionItemQualities.FindAsync(request.Id);

        if (foundQuality is null) return false;

        foundQuality.Name = request.Name;

        context.CollectionItemQualities.Update(foundQuality);
        await context.SaveChangesAsync();

        return true;
    }
}
