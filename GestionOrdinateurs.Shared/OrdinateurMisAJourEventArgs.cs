namespace GestionOrdinateurs.Shared;

// Donnees transportees quand l'evenement OrdinateurMisAJour se declenche
public class OrdinateurMisAJourEventArgs : EventArgs
{
    // Ordinateur avant la modification
    public Ordinateur AncienOrdinateur { get; }
    // Ordinateur après la modification
    public Ordinateur NouvelOrdinateur { get; }

    // Reçoit l'ancien et le nouvel ordinateur
    public OrdinateurMisAJourEventArgs(Ordinateur ancienOrdinateur, Ordinateur nouvelOrdinateur)
    {
        AncienOrdinateur = ancienOrdinateur;
        NouvelOrdinateur = nouvelOrdinateur;
    }
}
