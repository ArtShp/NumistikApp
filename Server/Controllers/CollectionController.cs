using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models.Collection;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionController(CollectionService collectionService) : MyControllerBase
{
    [HttpGet("my")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<List<CollectionDto.Response>>> GetAllMyCollections()
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var collections = await collectionService.GetAllMyCollectionsAsync(authenticatedUserId.Value);

        return Ok(collections);
    }

    [HttpGet("{collectionId}")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionDto.Response?>> GetCollection(Guid collectionId)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        // Get the user's role from claims
        UserAppRole? authenticatedUserRole = GetAuthorizedUserRole();

        if (authenticatedUserRole is null)
            return Unauthorized("User role is not recognized.");

        var collection = await collectionService.GetCollectionAsync(authenticatedUserId.Value, authenticatedUserRole.Value, collectionId);

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

    [HttpPost("role")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionDto.Response>> UpdateCollectionRole(CollectionUpdateRoleDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        bool success = await collectionService.UpdateCollectionRoleAsync(authenticatedUserId.Value, request);

        if (!success)
            return NotFound("Collection not found.");

        return Ok();
    }
}
