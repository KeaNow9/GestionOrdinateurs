using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// Contexte EF Core qui fait le lien avec la base SQLite
public class ParcInformatiqueContext : DbContext
{
    // Les options sont fournies par l'API ou par les tests
    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options) : base(options)
    {
    }

    // Table des réparations
    public DbSet<Reparation> Reparations { get; set; }


    // Table des ordinateurs
    public DbSet<Ordinateur> Ordinateurs { get; set; }
    // Table des utilisateurs
    public DbSet<User> Users { get; set; }
    // Configure la connexion a la base SQLite
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        optionsBuilder.UseSqlite("Data Source=parcinfo.db");
    }
}

    // Configuration des relations entre les entités
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Les caractéristiques sont stockées dans la table Ordinateur
        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(o => o.Caracteristiques);

        // Un ordinateur peut avoir plusieurs réparations
        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur);
            
        // Deux utilisateurs ne peuvent pas avoir le même login
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

    }
    
}
