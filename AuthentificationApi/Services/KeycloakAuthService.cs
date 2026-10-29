using System.Text.Json.Serialization;
using System.Text.Json;

namespace AuthentificationApi.Services;

// Service qui demande un token à Keycloak
public class KeycloakAuthService
{
    // Client HTTP pour appeler Keycloak
    private readonly HttpClient _httpClient;
    // Permet de lire appsettings.json (section Keycloak)
    private readonly IConfiguration _configuration;

    // Les services sont fournis par l'injection de dépendances
    public KeycloakAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    // Envoie le login et le mot de passe à Keycloak et renvoie le token
    public async Task<TokenResponse> LoginAsync(string username, string password)
    {
        var tokenEndpoint = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";

        // Données envoyées à Keycloak
        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", _configuration["Keycloak:ClientId"] },
            { "client_secret", _configuration["Keycloak:ClientSecret"] },
            { "username", username },
            { "password", password }
        };

        var content = new FormUrlEncodedContent(requestBody);

        var response = await _httpClient.PostAsync(tokenEndpoint, content);

        // Login correct : on lit le token
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResult = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            return tokenResult!;
        }

        // Login ou mot de passe incorrect
        throw new Exception("failed");
    }
}

// Réponse de Keycloak
public class TokenResponse
{
    // Token à envoyer aux API
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    // Durée de validité du token en secondes
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    // Token pour en redemander un nouveau
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
