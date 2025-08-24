using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.Continent;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContinentController(ContinentService continentService) : MyControllerBase
{
    [HttpGet]
    [AuthorizeAllUsers]
    public async Task<ActionResult<List<ContinentDto.Response>>> GetContinents()
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var continents = await continentService.GetContinentsAsync();

        return Ok(continents);
    }

    [HttpGet("{id:int}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<ContinentDto.Response?>> GetContinentAsync(int id)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var continent = await continentService.GetContinentAsync(id);

        if (continent is null)
            return NotFound("Continent not found.");

        return Ok(continent);
    }

    [HttpPost("create")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<ContinentCreationDto.Response?>> CreateContinentAsync(ContinentCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var continent = await continentService.CreateContinentAsync(request);

        if (continent is null)
            return BadRequest("Error creating continent.");

        return StatusCode(201, continent);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateContinentAsync(ContinentUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var continent = await continentService.UpdateContinentAsync(request);

        if (!continent)
            return BadRequest("Error updating continent.");

        return Ok();
    }
}
