// Création de l'application web
var builder = WebApplication.CreateBuilder(args);

// Enregistrement des controllers
builder.Services.AddControllers();

// Client HTTP pour appeler GestionOrdinateurs.API (HttpClientFactory)
builder.Services.AddHttpClient("ApiGestionOrdinateurs", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiGestionOrdinateurs:Url"] ?? "");
});

// Ajout de Swagger pour tester l'API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Autorise le client Blazor à appeler l'API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5199")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Construction de l'application
var app = builder.Build();

// Activer CORS
app.UseCors("AllowBlazorClient");

// Swagger seulement en développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Branchement des routes des controllers
app.MapControllers();

// Démarrage de l'API
app.Run();
