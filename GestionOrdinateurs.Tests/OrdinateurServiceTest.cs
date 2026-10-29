using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace GestionOrdinateurs.Tests;

// Tests du service des ordinateurs
public class OrdinateurServiceTest
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

    // Vérifie qu'un ordinateur ajouté se retrouve dans la liste
    [Fact]
    public async Task Ajouter_NouvelOrdinateur_EstDansLaListe()
    {
        var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);
        var ordinateur = new Ordinateur
        {
            Id = 100,
            Nom = "TestPC",
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable, "TestMarque", "TestCPU")
        };

        await service.Ajouter(ordinateur);

        var liste = await service.Lister();
        liste.Should().Contain(o => o.Id == 100);
    }

    // Vérifie que la suppression d'un ordinateur existant réussi
    [Fact]
    public async Task Supprimer_OrdinateurExistant_RetourneTrue()
    {
        var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);
        var ordinateur = new Ordinateur
        {
            Id = 101,
            Nom = "TestPC2",
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Fixe, "TestMarque", "TestCPU")
        };

        await service.Ajouter(ordinateur);

        var resultat = await service.Supprimer(101);

        resultat.Should().BeTrue();
        var liste = await service.Lister();
        liste.Should().NotContain(o => o.Id == 101);
    }

    // Vérifie que la suppression d'un ordinateur inexistant échoue
    [Fact]
    public async Task Supprimer_OrdinateurInexistant_RetourneFalse()
    {
        var contexte = CreerContexte();
        var service = new OrdinateurService(contexte);

        var resultat = await service.Supprimer(9999);

        resultat.Should().BeFalse();
    }
}
