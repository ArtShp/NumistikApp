using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.Country;

namespace Server.Services;

public class CountryService(MyDbContext context)
{
    public async Task<CountryCreationDto.Response?> CreateCountryAsync(CountryCreationDto.Request request)
    {
        var foundCountry = await context.Countries.FirstOrDefaultAsync(c => 
            c.Name == request.Name || c.Code == request.Code
        );

        if (foundCountry is not null) return null;

        var continent = await context.Continents.FindAsync(request.ContinentId);

        if (continent is null) return null;

        var currency = await context.Currencies.FindAsync(request.CurrencyId);

        if (currency is null) return null;

        var country = new Country
        {
            Name = request.Name,
            Code = request.Code,
            Continent = continent,
            Currency = currency
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
        if (request.CurrencyId.HasValue)
        {
            var currency = await context.Currencies.FindAsync(request.CurrencyId.Value);
            if (currency is null) return false;

            foundCountry.Currency = currency;
        }

        context.Countries.Update(foundCountry);
        await context.SaveChangesAsync();

        return true;
    }
}
