using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.Continent;

namespace Server.Services;

public class ContinentService(MyDbContext context)
{
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
