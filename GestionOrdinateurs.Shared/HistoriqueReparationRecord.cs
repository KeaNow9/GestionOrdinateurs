namespace GestionOrdinateurs.Shared;

// Une ligne de l'historique des réparations (qui a réparé quoi et quand)
public record HistoriqueReparationRecord(
    int OrdinateurId,
    string NomOrdinateur,
    string Technicien,
    string TypeReparation,
    DateTime DateIntervention
);
