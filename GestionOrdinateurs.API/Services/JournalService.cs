using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// Service qui écrit des messages dans des fichiers de log
public class JournalService
{
    // Verrou pour éviter que deux tâches écrivent dans le fichier en même temps
    private readonly object _verrou = new object();

    // Ajoute une ligne à la fin du fichier de log
    public void Ecrire(string dossier, string fichier, LigneJournal ligne)
    {
        lock (_verrou)
        {
            // Crée le dossier s'il n'existe pas
            Directory.CreateDirectory(dossier);

            // using ferme le fichier automatiquement à la fin
            using var writer = new StreamWriter(Path.Combine(dossier, fichier), append: true);
            writer.WriteLine(ligne.ToString());
        }
    }
}
