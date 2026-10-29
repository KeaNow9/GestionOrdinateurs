using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// Service qui gère les ordinateurs en base
public class OrdinateurService : IOrdinateurService
{
    // Accès à la base de données
    private readonly ParcInformatiqueContext _context;

    // Evènements déclenchés après un ajout ou une suppression ou une mise à jour
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    // Le context est fourni par l'injection de dépendances
    public OrdinateurService(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // Renvoie la liste complète des ordinateurs
    public async Task<List<Ordinateur>> Lister()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    // Renvoie un ordinateur par son ID ou null sinon
    public async Task<Ordinateur?> ObtenirParId(int id)
    {
        return await _context.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);
    }

    // Renvoie les ordinateurs selon la marque
    public async Task<List<Ordinateur>> RechercherParMarque(string marque)
    {
        return await _context.Ordinateurs
           .Where(o => o.Caracteristiques.Marque.ToLower() == marque.ToLower())
            .ToListAsync();
    }

    // Renvoie les ordinateurs disponibles (stock > 0)
    public async Task<List<Ordinateur>> ObtenirDisponibles()
    {
        return await _context.Ordinateurs
            .Where(o => o.Stock > 0)
            .ToListAsync();
    }

    // Renvoie les ordinateurs du plus cher au moins cher
    public async Task<List<Ordinateur>> ObtenirParPrixDescendant()
    {
        // SQLite ne sait pas trier les decimal donc on trie en mémoire
        var ordinateurs = await _context.Ordinateurs.ToListAsync();

        return ordinateurs
            .OrderByDescending(o => o.Prix)
            .ToList();
    }

    //Ajoute un ordinateur au parc
    public async Task Ajouter(Ordinateur ordinateur)
    {
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();

        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
    }

    //Met à jour un ordinateur existant renvoi false si pas présent
    public async Task<bool> MettreAJour(int id, Ordinateur modifie)
    {
        Ordinateur? existant = await ObtenirParId(id);

        if (existant == null)
        {
            return false;
        }

        // Copie de l'ancien ordinateur pour l'évènement
        var ancien = new Ordinateur
        {
            Id = existant.Id,
            Nom = existant.Nom,
            Etat = existant.Etat,
            Prix = existant.Prix,
            Stock = existant.Stock,
            Caracteristiques = existant.Caracteristiques
        };

        existant.Nom = modifie.Nom;
        existant.Etat = modifie.Etat;
        existant.Prix = modifie.Prix;
        existant.Stock = modifie.Stock;
        existant.Caracteristiques = modifie.Caracteristiques;
        await _context.SaveChangesAsync();

        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(ancien, existant));
        return true;
    }

    // Supprime un ordinateur renvoie false si inexistant
    public async Task<bool> Supprimer(int id)
    {
        Ordinateur? existant = await ObtenirParId(id);

        if (existant == null)
        {
            return false;
        }

        _context.Ordinateurs.Remove(existant);
        await _context.SaveChangesAsync();

        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(existant));
        return true;
    }

    // Donne l'action à faire selon l'état et les caractéristiques de l'ordinateur
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            // Ordinateur hors service : il faut le remplacer
            {
                Etat: EtatOrdinateur.HorsService
            }
                => "Remplacement",

            // Ordinateur lent avec moins de 8 Go de RAM : il faut ajouter de la RAM
            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques:
                {
                    RamGo: < 8
                }
            }
                => "Ajouter RAM",

            // Tous les autres cas
            _
                => "Aucune action"
        };
    }

    // Calcule les statistiques du parc et les renvoie dans un tuple nommé
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)
        CalculerStatistiques()
    {
        // SQLite ne sait pas additionner les decimal donc on calcule en mémoire
        var ordinateurs = _context.Ordinateurs.ToList();

        int totalOrdinateur = ordinateurs.Count;
        int ordiDisponibles = ordinateurs.Count(o => o.Stock > 0);
        decimal valeurStock = ordinateurs.Sum(o => o.Prix * o.Stock);
        int nbReparations = _context.Reparations.Count();

        return (totalOrdinateur, ordiDisponibles, valeurStock, nbReparations);
    }
}

// Données envoyées à l'API pour créer ou modifier un ordinateur (sans la liste des réparations)
public record OrdinateurRecord(
    string Nom,
    EtatOrdinateur Etat,
    int Stock,
    decimal Prix,
    Caracteristiques Caracteristiques
);
