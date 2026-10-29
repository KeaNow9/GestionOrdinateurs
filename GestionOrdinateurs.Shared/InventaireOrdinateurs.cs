namespace GestionOrdinateurs.Shared;

// Collection d'ordinateurs qu'on peut parcourir avec des indexeurs
public class InventaireOrdinateurs
{
    // Liste des ordinateurs de l'inventaire
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    // Indexeur par nom : inventaire["Dell XPS 15"] renvoie l'ordinateur ou null
    public Ordinateur? this[string nom]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase)); }
    }

    // Indexeur par identifiant : inventaire[3] renvoie l'ordinateur dont l'Id vaut 3 ou null
    public Ordinateur? this[int ordinateurId]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Id == ordinateurId); }
    }
}
