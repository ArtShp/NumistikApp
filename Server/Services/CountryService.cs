using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.Country;

namespace Server.Services;

public class CountryService(MyDbContext context)
{
    public IQueryable<CountryDto.Response> GetCountries()
    {
        return context.Countries.Select(ci =>
            new CountryDto.Response
            {
                Id = ci.Id,
                Name = ci.Name,
                Code = ci.Code,
                ContinentId = ci.ContinentId
            }
        );
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
            Code = country.Code,
            ContinentId = country.ContinentId
        };
    }

    public async Task<CountryCreationDto.Response?> CreateCountryAsync(CountryCreationDto.Request request)
    {
        var foundCountry = await context.Countries.FirstOrDefaultAsync(c => 
            c.Name == request.Name || c.Code == request.Code
        );

        if (foundCountry is not null) return null;

        var continent = await context.Continents.FindAsync(request.ContinentId);

        if (continent is null) return null;

        var country = new Country
        {
            Name = request.Name,
            Code = request.Code,
            Continent = continent
        };

        context.Countries.Add(country);
        await context.SaveChangesAsync();

        return new CountryCreationDto.Response
        {
            Id = country.Id,
            Name = country.Name,
            Code = country.Code
        };
    }

    public async Task<bool> UpdateCountryAsync(CountryUpdateDto.Request request)
    {
        var foundCountry = await context.Countries.FindAsync(request.Id);

        if (foundCountry is null) return false;

        if (request.Name is not null) foundCountry.Name = request.Name;
        if (request.Code is not null) foundCountry.Code = request.Code;
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
