using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AuthentificationApi.Models;
using AuthentificationApi.Services;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

// Controller des routes api/Authentication
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    // Factory de clients HTTP pour appeler GestionOrdinateurs.API
    private readonly IHttpClientFactory _httpClientFactory;
    // Permet de lire appsettings.json (clé JWT)
    private readonly IConfiguration _configuration;
    // Service qui demande les tokens à Keycloak
    private readonly KeycloakAuthService _keycloakAuthService;

    // Les services sont fournis par l'injection de dépendances
    public AuthenticationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, KeycloakAuthService keycloakAuthService)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _keycloakAuthService = keycloakAuthService;
    }

    // Génère un token JWT à partir du login de l'utilisateur
    [HttpPost]
    public async Task<IActionResult> GenererToken(string login)
    {
        // Recherche de l'utilisateur dans GestionOrdinateurs.API
        var client = _httpClientFactory.CreateClient("ApiGestionOrdinateurs");
        var reponse = await client.GetAsync($"api/Users/login/{login}");

        // Utilisateur inconnu : pas de token
        if (reponse.StatusCode == HttpStatusCode.NotFound)
            return NotFound("L'utilisateur n'existe pas");

        // Autre erreur de l'API des ordinateurs
        if (!reponse.IsSuccessStatusCode)
            return StatusCode((int)reponse.StatusCode);

        var user = await reponse.Content.ReadFromJsonAsync<User>();
        if (user == null)
            return NotFound("L'utilisateur n'existe pas");

        // Renvoie le token seul (comme dans la démo du cours)
        return Ok(CreerToken(user));
    }

    // Demande un token à Keycloak avec le login et le mot de passe
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _keycloakAuthService.LoginAsync(request.Username, request.Password);
            return Ok(token);
        }
        catch (Exception ex)
        {
            // Login ou mot de passe incorrect
            return Unauthorized(ex.Message);
        }
    }

    // Crée le token avec le login et le rôle de l'utilisateur valable 15 minutes
    private string CreerToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

        // Informations stockées dans le token (le rôle sert à [Authorize(Roles = "Admin")])
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
