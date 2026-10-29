namespace GestionOrdinateurs.Shared;

// Donnees transportees quand l'evenement OrdinateurAjoute se declenche
public class OrdinateurAjouteEventArgs : EventArgs
{
    // Ordinateur qui vient d'être ajouté
    public Ordinateur Ordinateur { get; }

    // Reçoit l'ordinateur ajouté
    public OrdinateurAjouteEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}
