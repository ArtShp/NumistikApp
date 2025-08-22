using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.CollectionItem;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionItemController(CollectionItemService collectionItemService) : MyControllerBase
{
    [HttpGet]
    [AuthorizeAllUsers]
    public ActionResult<IQueryable<CollectionItemDto.Response>> GetCollectionItems()
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        // Get the user's role from claims
        UserAppRole? authenticatedUserRole = GetAuthorizedUserRole();

        if (authenticatedUserRole is null)
            return Unauthorized("User role is not recognized.");

        var collectionItems = collectionItemService.GetCollectionItems(authenticatedUserId.Value, authenticatedUserRole.Value);

        return Ok(collectionItems);
    }

    [HttpGet("{id:int}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionItemDto.Response?>> GetCollectionItemAsync(int id)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        // Get the user's role from claims
        UserAppRole? authenticatedUserRole = GetAuthorizedUserRole();

        if (authenticatedUserRole is null)
            return Unauthorized("User role is not recognized.");

        var collectionItem = await collectionItemService.GetCollectionItemAsync(authenticatedUserId.Value, authenticatedUserRole.Value, id);

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

        var collectionItem = await collectionItemService.CreateCollectionItemAsync(request);

        if (collectionItem is null)
            return BadRequest("Error creating collection item.");

        return StatusCode(201, collectionItem);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCollectionItemAsync(CollectionItemUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var success = await collectionItemService.UpdateCollectionItemAsync(request);

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
