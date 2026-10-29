using GestionOrdinateurs.API;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

// Création de l'application web
var builder = WebApplication.CreateBuilder(args);

// Enregistrement des controllers
builder.Services.AddControllers();
// Enregistrement du context de la base
builder.Services.AddDbContext<ParcInformatiqueContext>();
// Enregistrement des services
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddScoped<ReparationService>();
// Une seule instance du stock pour toute l'API
builder.Services.AddSingleton<StockService>();
// Une seule instance du journal pour partager le même verrou
builder.Services.AddSingleton<JournalService>();
// Ajout de Swagger pour tester l'API
builder.Services.AddEndpointsApiExplorer();
// Swagger avec le bouton Authorize pour le token JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestionOrdinateurs.API", Version = "v1" });
    // Déclare le token JWT envoyé dans l'en-tête Authorization
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Collez seulement le token (Swagger ajoute Bearer)"
    });
    // Envoie le token avec chaque requête de Swagger
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Ancienne configuration JWT avec la même clé que AuthentificationApi (TP5)
// var jwtKey = builder.Configuration["Jwt:Key"] ?? "";
// var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    // Ancienne vérification avec la clé de AuthentificationApi (TP5)
    // options.TokenValidationParameters = new TokenValidationParameters
    // {
    //     ValidateIssuerSigningKey = true,
    //     IssuerSigningKey = new SymmetricSecurityKey(key),
    //     ValidateIssuer = false,
    //     ValidateAudience = false
    // };

    // Vérification des tokens Keycloak
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Le token doit venir du realm Keycloak
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Keycloak:Issuer"],

        // Le token doit être destiné à "account"
        ValidateAudience = true,
        ValidAudience = "account",

        ValidateIssuerSigningKey = true,
        ValidateLifetime = false,

        // Récupère les clés publiques de Keycloak pour vérifier la signature
        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            var client = new HttpClient();
            var keyUri = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}/protocol/openid-connect/certs";
            var response = client.GetAsync(keyUri).Result;
            var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);

            return keys.GetSigningKeys();
        }
    };
    // Affiche dans la console si le token est valide ou non
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Utilisateur utilisant un Token Valide --- {context.Principal?.Identity?.Name ?? "Anonymous"}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Token Invalide");
            return Task.CompletedTask;
        }
    };
});

// Enregistrement des autorisations (rôles)
builder.Services.AddAuthorization();

// Autorise le client Blazor à appeler l'API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Construction de l'application
var app = builder.Build();

//Initialisation de la base de données
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();
    
    db.Database.Migrate();

    if(!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Login="user1", Role=RoleEnum.Admin },
            new User { Login="user2", Role=RoleEnum.Gestionnaire },
            new User { Login="user3", Role=RoleEnum.Gestionnaire }
        );

        db.SaveChanges();
    }
}

// Swagger seulement en développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activer CORS (avant l'authentification)
app.UseCors("AllowBlazorClient");

// Authentification puis autorisation (l'ordre compte)
app.UseAuthentication();
app.UseAuthorization();

// Branchement des routes des controllers
app.MapControllers();

// Démarrage de l'API
app.Run();
