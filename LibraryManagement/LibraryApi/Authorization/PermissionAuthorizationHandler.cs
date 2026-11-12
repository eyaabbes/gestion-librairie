using LibraryApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryApi.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _context;

    public PermissionAuthorizationHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 1. Vérifier si l'utilisateur est Admin
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // 2. Récupérer l'ID de l'utilisateur connecté
        var userId = context.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return;

        // 3. Vérifier si l'utilisateur possède la permission demandée
        bool hasPermission = await _context.UserPermissions
            .AnyAsync(up =>
                up.UserId == userId &&
                up.Permission != null &&
                up.Permission.Name == requirement.PermissionName);

        // 4. Autoriser si la permission existe
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}