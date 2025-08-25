using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.CollectionItemQuality;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionItemQualityController(CollectionItemQualityService qualityService) : MyControllerBase
{
    [HttpGet]
    [AuthorizeAllUsers]
    public async Task<ActionResult<List<CollectionItemQualityDto.Response>>> GetCollectionItemQualities([FromQuery] int? lastSeenId)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var qualities = await qualityService.GetCollectionItemQualitiesAsync(lastSeenId, DefaultPageSize);

        return Ok(qualities);
    }

    [HttpGet("{id:int}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemQualityDto.Response?>> GetCollectionItemQualityAsync(int id)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var qualities = await qualityService.GetCollectionItemQualityAsync(id);

        if (qualities is null)
            return NotFound("Collection item quality not found.");

        return Ok(qualities);
    }

    [HttpPost("create")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<CollectionItemQualityCreationDto.Response?>> CreateCollectionItemQualityAsync(CollectionItemQualityCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var quality = await qualityService.CreateCollectionItemQualityAsync(request);

        if (quality is null)
            return BadRequest("Error creating quality for collection items.");

        return StatusCode(201, quality);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCollectionItemQualityAsync(CollectionItemQualityUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var success = await qualityService.UpdateCollectionItemQualityAsync(request);

        if (!success)
            return BadRequest("Error updating quality for collection items.");

        return Ok();
    }
}
