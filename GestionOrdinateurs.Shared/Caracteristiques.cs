namespace GestionOrdinateurs.Shared;

// Caractéristiques techniques d'un ordinateur
public class Caracteristiques
{
    // Quantité de RAM en Go
    public int RamGo { get; set; }

    // Type d'ordinateur (Portable ou Fixe ou Server)
    public TypeOrdinateur TypeOrdi { get; set; }

    // Taille du stockage en Go
    public int StockageGo { get; set; }

    // Marque de l'ordinateur
    public string Marque { get; set; }

    // Modèle du processeur
    public string Processeur { get; set; }

    // Les noms des paramètres doivent correspondre aux propriétés pour EF Core
    public Caracteristiques(int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string marque, string processeur)
    {
        RamGo = ramGo;
        StockageGo = stockageGo;
        TypeOrdi = typeOrdi;
        Marque = marque;
        Processeur = processeur;
    }
}
