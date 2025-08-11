using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionItemController(CollectionItemService collectionItemService) : MyControllerBase
{
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

        var catalogItem = await collectionItemService.UpdateCollectionItemAsync(request);

        if (!catalogItem)
            return BadRequest("Error updating collection item.");

        return Ok(); 
    }
}
