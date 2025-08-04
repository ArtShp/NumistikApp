using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models;
using Server.Services;
using System.Security.Claims;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CollectionController(CollectionService collectionService) : ControllerBase
{
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

    [HttpGet]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CollectionDto.Response>> GetAllCollections()
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var collections = await collectionService.GetAllCollectionsAsync(authenticatedUserId.Value);

        return Ok(collections);
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

    private Guid? GetAuthorizedUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;

        return userId;
    }
}
