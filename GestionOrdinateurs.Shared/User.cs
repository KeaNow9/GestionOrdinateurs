using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared;

// Utilisateur qui peut se connecter
public class User
{
    // Identifiant (clé primaire)
    public int Id { get; set; }

    // Login de connexion
    [Required]
    public string Login { get; set; } = string.Empty;

    // Rôle de l'utilisateur
    [Required]
    public RoleEnum Role { get; set; }
}