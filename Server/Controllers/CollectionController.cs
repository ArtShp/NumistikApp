using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.Collection;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionController(CollectionService collectionService) : MyControllerBase
{
    [HttpGet]
    [AuthorizeAllUsers]
    public ActionResult<IQueryable<CollectionDto.Response>> GetAllCollections()
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var collections = collectionService.GetAllCollections(authenticatedUserId.Value);

        return Ok(collections);
    }

    [HttpGet("{id}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionDto.Response>> GetCollection([FromRoute(Name = "id")] Guid collectionId)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var collection = await collectionService.GetCollectionAsync(authenticatedUserId.Value, collectionId);

        if (collection is null)
            return NotFound("Collection not found.");

        return Ok(collection);
    }

    [HttpPost("create")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionCreationDto.Response?>> CreateCollection(CollectionCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var collection = await collectionService.CreateCollectionAsync(authenticatedUserId.Value, request);

        if (collection is null)
            return BadRequest("Error creating collection.");

        return StatusCode(201, collection);
    }

    [HttpPost("assign-role")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionDto.Response>> AssignCollectionRole(CollectionAssignDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        bool collection = await collectionService.AssignCollectionRoleAsync(authenticatedUserId.Value, request);

        if (!collection)
            return NotFound("Collection not found.");

        return Ok();
    }
}
