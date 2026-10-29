namespace AuthentificationApi.Models;

// Login et mot de passe envoyés par la page de connexion
public class LoginRequest
{
    // Login de l'utilisateur
    public string Username { get; set; } = string.Empty;

    // Mot de passe de l'utilisateur
    public string Password { get; set; } = string.Empty;
}
