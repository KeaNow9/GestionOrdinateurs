namespace GestionOrdinateurs.API;

// Service qui gère le stock de pièces (SSD)
public class StockService
{
    // Stock de SSD disponible, 10 par défaut
    public int StockSSD { get; set; } = 10;

    // Objet servant de verrou pour protéger StockSSD des accès simultanés
    private readonly object _verrou = new object();

    // Consomme un SSD : petit délai puis on retire 1 au stock
    public void ConsommerSSD()
    {
        Task.Delay(10).Wait();

        // Un seul thread à la fois peut modifier le stock
        lock (_verrou)
        {
            if(StockSSD > 0)
            {
                StockSSD--;
            }
        }
    }
}
