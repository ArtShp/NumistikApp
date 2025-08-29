using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Services;
using Shared.Models.CollectionItemType;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionItemTypeController(CollectionItemTypeService typeService) : MyControllerBase
{
    [HttpGet]
    [AuthorizeAllUsers]
    public async Task<ActionResult<List<CollectionItemTypeDto.Response>>> GetCollectionItemTypes([FromQuery] int? lastSeenId)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var types = await typeService.GetCollectionItemTypesAsync(lastSeenId, DefaultPageSize);

        return Ok(types);
    }

    [HttpGet("{id:int}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemTypeDto.Response?>> GetCollectionItemTypeAsync(int id)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var types = await typeService.GetCollectionItemTypeAsync(id);

        if (types is null)
            return NotFound("Collection item type not found.");

        return Ok(types);
    }

    [HttpPost("create")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<CollectionItemTypeCreationDto.Response?>> CreateCollectionItemTypeAsync(CollectionItemTypeCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var type = await typeService.CreateCollectionItemTypeAsync(request);

        if (type is null)
            return BadRequest("Error creating type for collection items.");

        return StatusCode(201, type);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCollectionItemTypeAsync(CollectionItemTypeUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var success = await typeService.UpdateCollectionItemTypeAsync(request);

        if (!success)
            return BadRequest("Error updating type for collection items.");

        return Ok();
    }
}
