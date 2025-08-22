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
    public ActionResult<IQueryable<CollectionItemQualityDto.Response>> GetCollectionItemQualities()
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var qualitys = qualityService.GetCollectionItemQualities();

        return Ok(qualitys);
    }

    [HttpGet("{id:int}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemQualityDto.Response?>> GetCollectionItemQualityAsync(int id)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var qualitys = await qualityService.GetCollectionItemQualityAsync(id);

        if (qualitys is null)
            return NotFound("Collection item quality not found.");

        return Ok(qualitys);
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
