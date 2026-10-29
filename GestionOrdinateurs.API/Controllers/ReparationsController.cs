using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace GestionOrdinateurs.API.Controllers;

// Controller des routes api/reparations
[Authorize]
[ApiController]
[Route("api/[controller]")]


public class ReparationsController : ControllerBase
{
    // Service des réparations
    private readonly ReparationService _reparService;
    // Service des ordinateurs
    private readonly IOrdinateurService _ordiService;
    // Stock de pièces partagé par toute l'API (Singleton)
    private readonly StockService _stockService;
    // Permet d'écrire dans les logs
    private readonly ILogger<ReparationsController> _logger;

    // Les services sont fournis par l'injection de dépendances
    public ReparationsController(ReparationService reparService, IOrdinateurService ordiService, StockService stockService, ILogger<ReparationsController> logger)
    {
        _reparService = reparService;
        _ordiService = ordiService;
        _stockService = stockService;
        _logger = logger;
    }

    // Lance 5 réparations en même temps (réservé aux Admin)
    [Authorize(Roles = "Admin")]
    [HttpPost("SimulerReparation")]
    public async Task<IActionResult> SimulerReparation()
    {
        // Récupération de la liste d'ordinateurs
        var listOrdinateurs = await _ordiService.Lister();
        if (listOrdinateurs.Count < 5)
            return BadRequest("Il faut au moins 5 ordinateurs en base.");

        await Task.WhenAll(
            _reparService.SimulerReparationAsync(listOrdinateurs[0]),
            _reparService.SimulerReparationAsync(listOrdinateurs[1]),
            _reparService.SimulerReparationAsync(listOrdinateurs[2]),
            _reparService.SimulerReparationAsync(listOrdinateurs[3]),
            _reparService.SimulerReparationAsync(listOrdinateurs[4])
        );

        return Ok("5 réparations terminées");
    }

    // Liste les réparations avec leur ordinateur
    [HttpGet]
    public async Task<IActionResult> ListerReparations()
    {
        return Ok(await _reparService.ListerReparationsAsync());
    }

    // Renvoie l'historique des réparations
    [HttpGet("historique")]
    public async Task<IActionResult> ObtenirHistorique()
    {
        _logger.LogInformation("ObtenirHistorique appele");
        return Ok(await _reparService.ListerHistoriqueAsync());
    }

    // Consomme 10 SSD en parallèle (10 tâches en même temps) puis renvoie le stock restant (réservé aux Admin)
    [Authorize(Roles = "Admin")]
    [HttpGet("GetStockSSD")]
    public IActionResult GetStockSSD()
    {
        Parallel.For(
            0,
            10,
            i => _stockService.ConsommerSSD());

        _logger.LogInformation("StockSSD : {StockSSD}", _stockService.StockSSD);
        return Ok(_stockService.StockSSD);
    }
}
