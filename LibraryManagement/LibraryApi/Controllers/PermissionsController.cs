using LibraryApi.Models;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService) =>
        _permissionService = permissionService;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll() =>
        Ok((await _permissionService.GetAllAsync()).Select(ToResponse));

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var permission = await _permissionService.GetByIdAsync(id);
        return permission is null ? NotFound() : Ok(ToResponse(permission));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Permission permission)
    {
        try
        {
            var created = await _permissionService.CreateAsync(permission);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Permission permission)
    {
        try
        {
            return await _permissionService.UpdateAsync(id, permission)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        await _permissionService.DeleteAsync(id) ? NoContent() : NotFound();

    [HttpGet("users/{userId}/permissions")]
    public async Task<IActionResult> GetUserPermissions(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin") && !string.Equals(currentUserId, userId, StringComparison.Ordinal))
            return Forbid();

        try { return Ok(await _permissionService.GetUserPermissionNamesAsync(userId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPut("users/{userId}/permissions/{permissionId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Grant(string userId, int permissionId)
    {
        try
        {
            await _permissionService.GrantAsync(userId, permissionId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("users/{userId}/permissions/{permissionId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Revoke(string userId, int permissionId)
    {
        try
        {
            return await _permissionService.RevokeAsync(userId, permissionId)
                ? NoContent()
                : NotFound();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    private static object ToResponse(Permission permission) => new
    {
        permission.Id,
        permission.Name,
        permission.Description
    };
}
