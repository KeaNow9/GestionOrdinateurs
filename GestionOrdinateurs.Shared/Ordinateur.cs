using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;

// Ordinateur du parc (une seule entité pour tous les types)
[Table("Ordinateur")]
public class Ordinateur
{
    // Identifiant de l'ordinateur
    public int Id { get; set; }

    // Nom de l'ordinateur
    public string Nom { get; set; } = string.Empty;

    // Etat actuel de l'ordinateur
    public EtatOrdinateur Etat { get; set; }

    // Nombre d'ordinateurs en stock
    [DefaultValue(0)]
    public int Stock { get; set; }

    // Prix de l'ordinateur
    [DefaultValue(0)]
    public decimal Prix { get; set; }

    // Indique si l'ordinateur est vendu
    public bool Vendu { get; set; } = false;

    // Caractéristiques techniques (RAM stockage type marque processeur)
    public Caracteristiques Caracteristiques { get; set; }

    // Réparations faites sur cet ordinateur
    public List<Reparation> Reparations { get; set; } = new();
}
