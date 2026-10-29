using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

// Service qui gère les réparations des ordinateurs
public class ReparationService
{
    // Accès à la base de données
    private readonly ParcInformatiqueContext _context;

    // Permet de créer un context séparé pour chaque réparation
    private readonly IServiceScopeFactory _scopeFactory;

    // Écrit les logs dans des fichiers
    private readonly JournalService _journal;

    // Les services sont fournis automatiquement par l'API (injection de dépendances)
    public ReparationService(ParcInformatiqueContext context, IServiceScopeFactory scopeFactory, JournalService journal)
    {
        _context = context;
        _scopeFactory = scopeFactory;
        _journal = journal;
    }

    // Simule une réparation : création en base, attente de 2s, puis statut "Terminée"
    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        // Context à part, car plusieurs réparations tournent en même temps
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();

        // Création de la réparation en base
        var reparation = await CreerReparationAsync(context, ordinateur);

        // Simulation du temps de réparation
        await Task.Delay(2000);

        reparation.Statut = "Terminée";
        await context.SaveChangesAsync();
        Console.WriteLine($"{ordinateur.Nom} réparé");

        // Ligne de log avec la date du jour
        _journal.Ecrire("Logs/Reparations", "reparations.log", new LigneJournal(DateTime.Now, $"{ordinateur.Nom} réparé"));
    }

    // Crée une réparation "En cours" pour l'ordinateur et l'enregistre en base
    private async Task<Reparation> CreerReparationAsync(ParcInformatiqueContext context, Ordinateur ordinateur)
    {
        // On récupère l'ordinateur dans ce context pour pouvoir le lier à la réparation
        var ordi = await context.Ordinateurs.FindAsync(ordinateur.Id);

        var reparation = new Reparation
        {
            UnOrdinateur = ordi!,
            TypeReparation = "Diagnostic",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "test"
        };

        context.Reparations.Add(reparation);
        await context.SaveChangesAsync();
        return reparation;
    }

    // Liste les réparations avec l'ordinateur correspondant
    // On renvoie des records pour éviter la boucle Reparation -> Ordinateur -> Reparations dans le JSON
    public async Task<List<ReparationRecord>> ListerReparationsAsync()
    {
        return await _context.Reparations
            .Select(r => new ReparationRecord(
                r.Id,
                r.TypeReparation,
                r.DateDebut,
                r.Statut,
                r.UnOrdinateur.Id,
                r.UnOrdinateur.Nom,
                r.UnOrdinateur.Caracteristiques.Marque))
            .ToListAsync();
    }

    // Renvoie l'historique des réparations de la plus récente à la plus ancienne
    public async Task<List<HistoriqueReparationRecord>> ListerHistoriqueAsync()
    {
        return await _context.Reparations
            .OrderByDescending(r => r.DateDebut)
            .Select(r => new HistoriqueReparationRecord(
                r.UnOrdinateur.Id,
                r.UnOrdinateur.Nom,
                r.Technicien,
                r.TypeReparation,
                r.DateDebut))
            .ToListAsync();
    }
}

// Données d'une réparation renvoyées par l'API avec les infos de son ordinateur
public record ReparationRecord(
    int Id,
    string TypeReparation,
    DateTime DateDebut,
    string Statut,
    int OrdinateurId,
    string NomOrdinateur,
    string MarqueOrdinateur
);
