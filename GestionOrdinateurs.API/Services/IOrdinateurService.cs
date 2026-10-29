using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// Contrat du service : ce qu'on peut faire avec les ordinateurs
public interface IOrdinateurService
{
    // Evenements declenches par le service quand il ajoute/supprime/modifie un ordinateur
    event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    // Renvoie tous les ordinateurs
    Task<List<Ordinateur>> Lister();
    // Renvoie un ordinateur par son ID
    Task<Ordinateur?> ObtenirParId(int id);
    // Renvoie les ordinateurs d'une marque
    Task<List<Ordinateur>> RechercherParMarque(string marque);
    // Renvoie les ordinateurs en stock
    Task<List<Ordinateur>> ObtenirDisponibles();
    // Renvoie les ordinateurs du plus cher au moins cher
    Task<List<Ordinateur>> ObtenirParPrixDescendant();
    // Ajoute un ordinateur
    Task Ajouter(Ordinateur ordinateur);
    // Modifie un ordinateur existant
    Task<bool> MettreAJour(int id, Ordinateur modifie);
    // Supprime un ordinateur
    Task<bool> Supprimer(int id);
    // Donne l'action à faire sur un ordinateur
    string Diagnostiquer(Ordinateur ordinateur);
    // Calcule les statistiques du parc
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();
}
