using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionOrdinateurs.Client;
using Blazored.SessionStorage;
using GestionOrdinateurs.Client.Utils;

// Création de l'application Blazor
var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Composants racines de la page
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Client HTTP par défaut
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Stockage du token en session
builder.Services.AddBlazoredSessionStorage();

// Client HTTP vers l'API d'authentification (HttpClientFactory)
builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5157/");
});

// Handler qui ajoute le token JWT aux requêtes
builder.Services.AddScoped<JwtHandler>();

// Client HTTP vers l'API des ordinateurs avec le token (HttpClientFactory)
builder.Services.AddHttpClient("ApiGestionOrdinateurs", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010/api/");
})
.AddHttpMessageHandler<JwtHandler>();

// Démarrage de l'application
await builder.Build().RunAsync();
