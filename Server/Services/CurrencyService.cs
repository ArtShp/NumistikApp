using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Models.Currency;

namespace Server.Services;

public class CurrencyService(MyDbContext context)
{
    public IQueryable<CurrencyDto.Response> GetCurrencies()
    {
        return context.Currencies.Select(ci =>
            new CurrencyDto.Response
            {
                Id = ci.Id,
                MajorName = ci.MajorName,
                MinorName = ci.MinorName,
                Code = ci.Code,
                Symbol = ci.Symbol
            }
        );
    }

    public async Task<CurrencyDto.Response?> GetCurrencyAsync(int currencyId)
    {
        var currency = await context.Currencies
            .FindAsync(currencyId);

        if (currency is null) return null;

        return new CurrencyDto.Response
        {
            Id = currency.Id,
            MajorName = currency.MajorName,
            MinorName = currency.MinorName,
            Code = currency.Code,
            Symbol = currency.Symbol
        };
    }

    public async Task<CurrencyCreationDto.Response?> CreateCurrencyAsync(CurrencyCreationDto.Request request)
    {
        var foundCurrency = await context.Currencies.FirstOrDefaultAsync(c => c.Code == request.Code);

        if (foundCurrency is not null) return null;

        var currency = new Currency
        {
            MajorName = request.MajorName,
            MinorName = request.MinorName,
            Code = request.Code,
            Symbol = request.Symbol
        };

        context.Currencies.Add(currency);
        await context.SaveChangesAsync();

        return new CurrencyCreationDto.Response
        {
            Id = currency.Id,
            MajorName = currency.MajorName,
            Code = currency.Code
        };
    }

    public async Task<bool> UpdateCurrencyAsync(CurrencyUpdateDto.Request request)
    {
        var foundCurrency = await context.Currencies.FindAsync(request.Id);

        if (foundCurrency is null) return false;

        if (request.MajorName is not null) foundCurrency.MajorName = request.MajorName;
        if (request.MinorName is not null) foundCurrency.MinorName = request.MinorName;
        if (request.Code is not null) foundCurrency.Code = request.Code;
        if (request.Symbol is not null) foundCurrency.Symbol = request.Symbol;

        context.Currencies.Update(foundCurrency);
        await context.SaveChangesAsync();

        return true;
    }
}
