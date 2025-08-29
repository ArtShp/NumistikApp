using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.CollectionItem;
using Server.Models.Common;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionItemController(CollectionItemService collectionItemService) : MyControllerBase
{
    [HttpGet("{collectionId:Guid}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<List<CollectionItemDto.Response>?>> GetCollectionItemsAsync(Guid collectionId, [FromQuery] int? lastSeenId)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        // Get the user's role from claims
        UserAppRole? authenticatedUserRole = GetAuthorizedUserRole();

        if (authenticatedUserRole is null)
            return Unauthorized("User role is not recognized.");

        var collectionItems = await collectionItemService
            .GetCollectionItemsAsync(authenticatedUserId.Value, authenticatedUserRole.Value, collectionId, lastSeenId, DefaultPageSize);

        if (collectionItems is null)
            return NotFound("No collection items found.");

        return Ok(collectionItems);
    }

    [HttpGet("{collectionId:Guid}/{itemId:int}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemDto.Response?>> GetCollectionItemAsync(Guid collectionId, int itemId)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        // Get the user's role from claims
        UserAppRole? authenticatedUserRole = GetAuthorizedUserRole();

        if (authenticatedUserRole is null)
            return Unauthorized("User role is not recognized.");

        var collectionItem = await collectionItemService
            .GetCollectionItemAsync(authenticatedUserId.Value, authenticatedUserRole.Value, collectionId, itemId);

        if (collectionItem is null)
            return NotFound("Collection item not found.");

        return Ok(collectionItem);
    }

    [HttpPost("create")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemCreationDto.Response?>> CreateCollectionItemAsync(CollectionItemCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var collectionItem = await collectionItemService.CreateCollectionItemAsync(authenticatedUserId.Value, request);

        if (collectionItem is null)
            return BadRequest("Error creating collection item.");

        return StatusCode(201, collectionItem);
    }

    [HttpPost("update")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<bool>> UpdateCollectionItemAsync(CollectionItemUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var success = await collectionItemService.UpdateCollectionItemAsync(authenticatedUserId.Value, request);

        if (!success)
            return BadRequest("Error updating collection item.");

        return Ok(); 
    }

    [HttpGet("image/{filename}")]
    [AuthorizeAllUsers]
    public IResult GetImageAsync(string filename)
    {
        var filePath = collectionItemService.GetImagePath(filename);

        if (filePath is null)
            return TypedResults.NotFound("No file found with the provided file name.");

        return TypedResults.PhysicalFile(filePath, fileDownloadName: filename);
    }
}
