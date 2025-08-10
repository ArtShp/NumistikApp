using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogItemController(CatalogItemService catalogItemService) : MyControllerBase
{
    [HttpPost("create")]
    [AuthorizeAllUsers]
    public async Task<ActionResult<CatalogItemCreationDto.Response?>> CreateCatalogItemAsync(CatalogItemCreationDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var catalogItem = await catalogItemService.CreateCatalogItemAsync(request);

        if (catalogItem is null)
            return BadRequest("Error creating catalog item.");

        return StatusCode(201, catalogItem);
    }

    [HttpPost("update")]
    [AuthorizeOnlyAdmins]
    public async Task<ActionResult<bool>> UpdateCatalogItemAsync(CatalogItemUpdateDto.Request request)
    {
        // Get the user's id from claims
        Guid? authenticatedUserId = GetAuthorizedUserId();

        if (authenticatedUserId is null)
            return Unauthorized("User is not authenticated.");

        var catalogItem = await catalogItemService.UpdateCatalogItemAsync(request);

        if (!catalogItem)
            return BadRequest("Error updating catalog item.");

        return Ok();
    }
}
