using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Controllers;

// Controller des routes api/Users
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Accès à la base de données
    private readonly ParcInformatiqueContext _context;

    // Le context est fourni par l'injection de dépendances
    public UsersController(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // Récupérer tous les utilisateurs
    [HttpGet]
    public async Task<IActionResult> Lister()
    {
        return Ok(await _context.Users.ToListAsync());
    }

    // Chercher un utilisateur par son login
    [HttpGet("login/{login}")]
    public async Task<IActionResult> ObtenirParLogin(string login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null) return NotFound();
        return Ok(user);
    }
}
