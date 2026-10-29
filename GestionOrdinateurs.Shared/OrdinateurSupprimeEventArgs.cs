namespace GestionOrdinateurs.Shared;

// Donnees transportees quand l'evenement OrdinateurSupprime se declenche
public class OrdinateurSupprimeEventArgs : EventArgs
{
    // Ordinateur qui vient d'être supprimé
    public Ordinateur Ordinateur { get; }

    // Reçoit l'ordinateur supprimé
    public OrdinateurSupprimeEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}
