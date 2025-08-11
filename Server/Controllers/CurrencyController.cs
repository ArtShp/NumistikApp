using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.Currency;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CurrencyController(CurrencyService currencyService) : MyControllerBase
{
    [HttpPost("create")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CurrencyCreationDto.Response?>> CreateCurrencyAsync(CurrencyCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var currency = await currencyService.CreateCurrencyAsync(request);

        if (currency is null)
            return BadRequest("Error creating currency.");

        return StatusCode(201, currency);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCurrencyAsync(CurrencyUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var currency = await currencyService.UpdateCurrencyAsync(request);

        if (!currency)
            return BadRequest("Error updating currency.");

        return Ok();
    }
}
