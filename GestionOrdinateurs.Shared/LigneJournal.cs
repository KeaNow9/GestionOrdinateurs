namespace GestionOrdinateurs.Shared;

// Une ligne du journal : petite donnée qui ne change pas donc une structure suffit
public readonly struct LigneJournal
{
    // Date de l'action
    public DateTime Date { get; }

    // Description de l'action
    public string Message { get; }

    // Crée une ligne avec sa date et son message
    public LigneJournal(DateTime date, string message)
    {
        Date = date;
        Message = message;
    }

    // Format écrit dans le fichier : 15/10/2026 - Dell XPS réparé
    public override string ToString()
    {
        return $"{Date:dd/MM/yyyy} - {Message}";
    }
}
