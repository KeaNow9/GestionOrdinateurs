// Création de la Gateway
var builder = WebApplication.CreateBuilder(args);

// Ajout de OpenAPI
builder.Services.AddOpenApi();

// Ajout de YARP avec les routes de appsettings.json
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// Autorise le client Blazor à appeler la Gateway
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Activer CORS
app.UseCors("AllowBlazorClient");

// OpenAPI seulement en développement
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redirection vers HTTPS
app.UseHttpsRedirection();

// Transfère les requêtes vers les API
app.MapReverseProxy();

// Exemple météo du modèle
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Démarrage de la Gateway
app.Run();

// Données météo du modèle
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
