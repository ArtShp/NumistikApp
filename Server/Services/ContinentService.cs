using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.Continent;

namespace Server.Services;

public class ContinentService(MyDbContext context)
{
    public async Task<List<ContinentDto.Response>> GetContinentsAsync(int? lastSeenId, int pageSize)
    {
        IQueryable<Continent> query = context.Continents
            .OrderBy(c => c.Id);

        if (lastSeenId.HasValue)
        {
            query = query.Where(c => c.Id > lastSeenId.Value);
        }

        return await query.Take(pageSize).Select(ci =>
            new ContinentDto.Response
            {
                Id = ci.Id,
                Name = ci.Name
            })
            .ToListAsync();
    }

    public async Task<ContinentDto.Response?> GetContinentAsync(int continentId)
    {
        var continent = await context.Continents
            .FindAsync(continentId);

        if (continent is null) return null;

        return new ContinentDto.Response
        {
            Id = continent.Id,
            Name = continent.Name
        };
    }

    public async Task<ContinentCreationDto.Response?> CreateContinentAsync(ContinentCreationDto.Request request)
    {
        var foundContinent = await context.Continents.FirstOrDefaultAsync(c => c.Name == request.Name);

        if (foundContinent is not null) return null;

        var continent = new Continent
        {
            Name = request.Name
        };

        context.Continents.Add(continent);
        await context.SaveChangesAsync();

        return new ContinentCreationDto.Response
        {
            Id = continent.Id,
            Name = continent.Name
        };
    }

    public async Task<bool> UpdateContinentAsync(ContinentUpdateDto.Request request)
    {
        var foundContinent = await context.Continents.FirstOrDefaultAsync(c => c.Id == request.Id);

        if (foundContinent is null) return false;

        foundContinent.Name = request.Name;

        context.Continents.Update(foundContinent);
        await context.SaveChangesAsync();

        return true;
    }
}
