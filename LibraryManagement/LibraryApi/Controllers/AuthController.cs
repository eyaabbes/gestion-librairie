using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // 1. Chercher l'utilisateur par son email
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        // 2. Vérifier le mot de passe
        var passwordValid = await _userManager.CheckPasswordAsync(
            user, dto.Password);

        if (!passwordValid)
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });

        // 3. Récupérer les rôles de l'utilisateur
        var roles = await _userManager.GetRolesAsync(user);

        // 4. Construire les Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 5. Récupérer la clé JWT
        var key = _configuration["Jwt:Key"]!;

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        // 6. Définir la durée du token
        var duration = Convert.ToDouble(
            _configuration["Jwt:DurationInMinutes"]);

        // 7. Créer le token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(duration),
            signingCredentials: credentials
        );

        // 8. Convertir le token en chaîne
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // 9. Retourner le token
        return Ok(new
        {
            message = "Connexion réussie.",
            token = tokenString,
            expiration = token.ValidTo,
            email = user.Email,
            fullName = user.FullName,
            roles = roles
        });
    }
}