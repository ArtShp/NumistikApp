using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.Country;

namespace Server.Services;

public class CountryService(MyDbContext context)
{
    public async Task<List<CountryDto.Response>> GetCountriesAsync(string? lastSeenName, int pageSize)
    {
        IQueryable<Country> query = context.Countries
            .OrderBy(c => c.Name);

        if (!string.IsNullOrEmpty(lastSeenName))
        {
            query = query.Where(c => string.Compare(c.Name, lastSeenName) > 0);
        }

        return await query.Take(pageSize).Select(ci =>
            new CountryDto.Response
            {
                Id = ci.Id,
                Name = ci.Name,
                ContinentId = ci.ContinentId
            })
            .ToListAsync();
    }

    public async Task<CountryDto.Response?> GetCountryAsync(int countryId)
    {
        var country = await context.Countries
            .FindAsync(countryId);

        if (country is null) return null;

        return new CountryDto.Response
        {
            Id = country.Id,
            Name = country.Name,
            ContinentId = country.ContinentId
        };
    }

    public async Task<CountryCreationDto.Response?> CreateCountryAsync(CountryCreationDto.Request request)
    {
        var foundCountry = await context.Countries.FirstOrDefaultAsync(c => 
            c.Name == request.Name
        );

        if (foundCountry is not null) return null;

        var continent = await context.Continents.FindAsync(request.ContinentId);

        if (continent is null) return null;

        var country = new Country
        {
            Name = request.Name,
            Continent = continent
        };

        context.Countries.Add(country);
        await context.SaveChangesAsync();

        return new CountryCreationDto.Response
        {
            Id = country.Id,
            Name = country.Name
        };
    }

    public async Task<bool> UpdateCountryAsync(CountryUpdateDto.Request request)
    {
        var foundCountry = await context.Countries.FindAsync(request.Id);

        if (foundCountry is null) return false;

        if (request.Name is not null) foundCountry.Name = request.Name;
        if (request.ContinentId.HasValue)
        {
            var continent = await context.Continents.FindAsync(request.ContinentId.Value);
            if (continent is null) return false;
            
            foundCountry.Continent = continent;
        }

        context.Countries.Update(foundCountry);
        await context.SaveChangesAsync();

        return true;
    }
}
