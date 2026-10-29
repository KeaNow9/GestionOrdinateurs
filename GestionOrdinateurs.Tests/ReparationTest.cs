using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace GestionOrdinateurs.Tests;

// Tests sur les réparations
public class ReparationTest
{
    // Crée un context sur une base en mémoire (nouvelle base vide pour chaque test)
    private ParcInformatiqueContext CreerContexte()
    {
        var options =
            new DbContextOptionsBuilder<ParcInformatiqueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ParcInformatiqueContext(options);
    }

    // Vérifie qu'une réparation créée est bien enregistrée et liée à son ordinateur
    [Fact]
    public async Task CreerReparation_AvecOrdinateur_EstEnregistree()
    {
        var contexte = CreerContexte();
        var ordinateur = new Ordinateur
        {
            Nom = "TestPC",
            Caracteristiques = new Caracteristiques(8, 256, TypeOrdinateur.Portable, "TestMarque", "TestCPU")
        };
        var reparation = new Reparation
        {
            UnOrdinateur = ordinateur,
            TypeReparation = "Diagnostic",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "test"
        };

        contexte.Ordinateurs.Add(ordinateur);
        contexte.Reparations.Add(reparation);
        await contexte.SaveChangesAsync();

        var reparations = await contexte.Reparations.Include(r => r.UnOrdinateur).ToListAsync();
        reparations.Should().HaveCount(1);
        reparations[0].UnOrdinateur.Nom.Should().Be("TestPC");
        reparations[0].Technicien.Should().Be("test");
        ordinateur.Reparations.Should().Contain(reparation);
    }
}
