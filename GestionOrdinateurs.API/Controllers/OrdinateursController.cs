using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;


namespace GestionOrdinateurs.API.Controllers;

// Controller des routes api/Ordinateurs
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    // Service des ordinateurs
    private readonly IOrdinateurService _ordiService;
    // Permet d'écrire dans les logs
    private readonly ILogger<OrdinateursController> _logger;
    // Permet de lire appsettings.json
    private readonly IConfiguration _configuration;
    // Écrit les logs dans des fichiers
    private readonly JournalService _journal;


    // Les services sont fournis par l'injection de dépendances
    public OrdinateursController(IOrdinateurService service, ILogger<OrdinateursController> logger, IConfiguration configuration, JournalService journal)
    {
        _ordiService = service;
        _logger = logger;
        _configuration = configuration;
        _journal = journal;

        // Abonnement aux évènements du service
        _ordiService.OrdinateurAjoute += AfficherMessageAjout;
        _ordiService.OrdinateurSupprime += AfficherMessageSuppression;
        _ordiService.OrdinateurMisAJour += AfficherMessageMiseAJour;
    }

    // Methode appelee quand un ordinateur est ajoute, affiche juste un message
    private void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        Console.WriteLine($"[Evenement] Ajout de {e.Ordinateur.Nom} ({e.Ordinateur.Caracteristiques.Marque})");
        EcrireLogOrdinateur($"Ajout de {e.Ordinateur.Nom} ({e.Ordinateur.Caracteristiques.Marque})");
    }

    // Methode appelee quand un ordinateur est supprime
    private void AfficherMessageSuppression(object? sender, OrdinateurSupprimeEventArgs e)
    {
        Console.WriteLine($"[Evenement] Suppression de {e.Ordinateur.Nom} ({e.Ordinateur.Caracteristiques.Marque})");
        EcrireLogOrdinateur($"Suppression de {e.Ordinateur.Nom} ({e.Ordinateur.Caracteristiques.Marque})");
    }

    // Methode appelee quand un ordinateur est mis a jour, on recoit l'ancien et le nouveau
    private void AfficherMessageMiseAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        Console.WriteLine($"[Evenement] Mise à jour de {e.AncienOrdinateur.Nom} devient {e.NouvelOrdinateur.Nom}");
        EcrireLogOrdinateur($"Modification de {e.AncienOrdinateur.Nom} devient {e.NouvelOrdinateur.Nom}");
    }

    // Écrit une action sur un ordinateur dans le fichier de log avec la date du jour
    private void EcrireLogOrdinateur(string action)
    {
        _journal.Ecrire("Logs/Ordinateurs", "ordinateurs.log", new LigneJournal(DateTime.Now, action));
    }

    // Lister tous les ordinateurs
    [HttpGet]
    public async Task<IActionResult> Lister()
    {
        _logger.LogInformation("Lister appele");
        return Ok(await _ordiService.Lister());
    }

    // Renvoyer un ordinateur par son ID
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenirParId(int id)
    {
        _logger.LogInformation("ObtenirParId appele avec id {Id}", id);
        var ordinateur = await _ordiService.ObtenirParId(id);
        if (ordinateur == null) return NotFound();
        return Ok(ordinateur);
    }

    // Rechercher les ordinateurs par marque
    [HttpGet("marque/{marque}")]
    public async Task<IActionResult> RechercherParMarque(string marque)
    {
        _logger.LogInformation("RechercherParMarque appele avec marque {Marque}", marque);
        return Ok(await _ordiService.RechercherParMarque(marque));
    }

    // Renvoyer les ordinateurs disponibles (stock > 0)
    [HttpGet("disponibles")]
    public async Task<IActionResult> ObtenirDisponibles()
    {
        _logger.LogInformation("ObtenirDisponibles appele");
        return Ok(await _ordiService.ObtenirDisponibles());
    }

    // Renvoyer les ordinateurs du plus cher au moins cher
    [HttpGet("par-prix")]
    public async Task<IActionResult> ObtenirParPrixDescendant()
    {
        _logger.LogInformation("ObtenirParPrixDescendant appele");
        return Ok(await _ordiService.ObtenirParPrixDescendant());
    }

    // Renvoyer le diagnostic d'un ordinateur
    [HttpGet("{id}/diagnostic")]
    public async Task<IActionResult> Diagnostiquer(int id)
    {
        _logger.LogInformation("Diagnostiquer appele avec id {Id}", id);
        var ordinateur = await _ordiService.ObtenirParId(id);
        if (ordinateur == null) return NotFound();

        var diagnostic = _ordiService.Diagnostiquer(ordinateur);
        return Ok(new { ordinateur.Id, ordinateur.Nom, ordinateur.Etat, diagnostic });
    }

    // Renvoyer les statistiques du parc
    [HttpGet("statistiques")]
    public IActionResult ObtenirStatistiques()
    {
        _logger.LogInformation("ObtenirStatistiques appele");

        // Déconstruction du tuple dans 4 variables
        var (total, enStock, valeurStock, nbReparations)
            = _ordiService.CalculerStatistiques();

        return Ok(new { total, enStock, valeurStock, nbReparations });
    }

    // Chercher un ordinateur par son nom dans l'inventaire
    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> ChercherDansInventaire(string nom)
    {
        _logger.LogInformation("ChercherDansInventaire appele avec nom {Nom}", nom);
        var inventaire = new InventaireOrdinateurs();
        inventaire.Ordinateurs = await _ordiService.Lister();

        // Recherche par nom grâce à l'indexeur
        var ordinateur = inventaire[nom];
        if (ordinateur == null) return NotFound();

        return Ok(ordinateur);
    }

    // Ajouter un ordinateur (réservé aux Admin)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Ajouter(OrdinateurRecord donnees)
    {
        _logger.LogInformation("Ajouter appele");
        var ordinateur = ConvertirEnOrdinateur(donnees);
        await _ordiService.Ajouter(ordinateur);
        return StatusCode(201, ordinateur);
    }

    // Modifier un ordinateur existant (réservé aux Admin)
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> MettreAJour(int id, OrdinateurRecord donnees)
    {
        _logger.LogInformation("MettreAJour appele avec id {Id}", id);
        var success = await _ordiService.MettreAJour(id, ConvertirEnOrdinateur(donnees));
        if (!success) return NotFound();
        return Ok();
    }

    // Transforme les données reçues en Ordinateur
    private static Ordinateur ConvertirEnOrdinateur(OrdinateurRecord donnees)
    {
        // Déconstruction du record dans des variables
        var (nom, etat, stock, prix, caracteristiques) = donnees;

        return new Ordinateur
        {
            Nom = nom,
            Etat = etat,
            Stock = stock,
            Prix = prix,
            Caracteristiques = caracteristiques
        };
    }

    // Supprimer un ordinateur (réservé aux Admin)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        _logger.LogInformation("Supprimer appele avec id {Id}", id);
        var success = await _ordiService.Supprimer(id);
        if (!success) return NotFound();
        return NoContent();
    }

    // Afficher les informations de l'application
    [HttpGet("info")]
    public IActionResult ObtenirInfosApplication()
    {
        _logger.LogInformation("ObtenirInfosApplication appele");
        var nom = _configuration["Application:Name"];
        var version = _configuration["Application:Version"];
        return Ok(new { nom, version });
    }
}
