using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.CollectionItemSpecialStatus;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionItemSpecialStatusController(CollectionItemSpecialStatusService specialStatusService) : MyControllerBase
{
    [HttpPost("create")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemSpecialStatusCreationDto.Response?>> CreateCollectionItemSpecialStatusAsync(CollectionItemSpecialStatusCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var specialStatus = await specialStatusService.CreateCollectionItemSpecialStatusAsync(request);

        if (specialStatus is null)
            return BadRequest("Error creating special status for collection items.");

        return StatusCode(201, specialStatus);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCollectionItemSpecialStatusAsync(CollectionItemSpecialStatusUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var continent = await specialStatusService.UpdateCollectionItemSpecialStatusAsync(request);

        if (!continent)
            return BadRequest("Error updating special status for collection items.");

        return Ok();
    }
}
