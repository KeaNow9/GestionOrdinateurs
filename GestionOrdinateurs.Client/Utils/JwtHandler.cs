using System.Net.Http.Headers;
using Blazored.SessionStorage;

namespace GestionOrdinateurs.Client.Utils;

// Ajoute le token JWT dans chaque requête envoyée à l'API
public class JwtHandler : DelegatingHandler
{
    // Stockage de session où le token est gardé
    private readonly ISessionStorageService _sessionStorage;

    // Le stockage est fourni par l'injection de dépendances
    public JwtHandler(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    // Appelé avant chaque requête HTTP
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Lecture du token enregistré à la connexion
        var token = await _sessionStorage.GetItemAsync<string>("jwtToken");

        // Si connecté on ajoute l'en-tête Authorization: Bearer {token}
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // On laisse la requête continuer
        return await base.SendAsync(request, cancellationToken);
    }
}
