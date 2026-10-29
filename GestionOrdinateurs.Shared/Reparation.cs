namespace GestionOrdinateurs.Shared;

// Réparation faite sur un ordinateur
public class Reparation
{
    // nom technicien 
    public string Technicien { get; set; }
    // Identifiant de la réparation
    public int Id { get; set; }

    // Ordinateur concerné par la réparation
    public Ordinateur UnOrdinateur { get; set; }

    // Type de réparation effectuée
    public string TypeReparation { get; set; }

    // Date de début de la réparation
    public DateTime DateDebut { get; set; }

    // Statut de la réparation 
    public string Statut { get; set; }
}
