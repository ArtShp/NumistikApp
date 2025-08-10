using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController(CountryService countryService) : MyControllerBase
{
    [HttpPost("create")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CountryCreationDto.Response?>> CreateCountryAsync(CountryCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var country = await countryService.CreateCountryAsync(request);

        if (country is null)
            return BadRequest("Error creating country.");

        return StatusCode(201, country);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCountryAsync(CountryUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var country = await countryService.UpdateCountryAsync(request);

        if (!country)
            return BadRequest("Error updating country.");

        return Ok();
    }
}
