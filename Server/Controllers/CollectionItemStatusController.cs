using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.CollectionItemStatus;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionItemStatusController(CollectionItemStatusService statusService) : MyControllerBase
{
    [HttpGet]
    [AuthorizeAllUsers]
    public async Task<ActionResult<List<CollectionItemStatusDto.Response>>> GetCollectionItemStatuses()
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var statuss = await statusService.GetCollectionItemStatusesAsync();

        return Ok(statuss);
    }

    [HttpGet("{id:int}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemStatusDto.Response?>> GetCollectionItemStatusAsync(int id)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var statuss = await statusService.GetCollectionItemStatusAsync(id);

        if (statuss is null)
            return NotFound("Collection item status not found.");

        return Ok(statuss);
    }

    [HttpPost("create")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<CollectionItemStatusCreationDto.Response?>> CreateCollectionItemStatusAsync(CollectionItemStatusCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var status = await statusService.CreateCollectionItemStatusAsync(request);

        if (status is null)
            return BadRequest("Error creating status for collection items.");

        return StatusCode(201, status);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCollectionItemStatusAsync(CollectionItemStatusUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var success = await statusService.UpdateCollectionItemStatusAsync(request);

        if (!success)
            return BadRequest("Error updating status for collection items.");

        return Ok();
    }
}
