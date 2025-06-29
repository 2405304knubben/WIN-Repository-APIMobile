// Dit bestand beschrijft wat een product is
// Een product is iets wat klanten kunnen kopen, zoals een speelgoed of een boek
namespace MauiApp1.ModelAPI
{
    // Een Product is iets wat we verkopen
    public class Product
    {
        // Elk product heeft een eigen nummer (Id)
        public int Id { get; set; }
        
        // Dit is de naam van het product
        // Bijvoorbeeld: "Rode Fiets" of "Blauwe Bal"
        public string Name { get; set; } = "";
        
        // Dit vertelt meer over het product
        // Bijvoorbeeld: "Een mooie rode fiets voor kinderen"
        public string Description { get; set; } = "";
        
        // Dit is hoeveel het product kost
        // Bijvoorbeeld: 12.99 euro
        public decimal Price { get; set; }
        
        // Dit zijn alle bestellingen waar dit product in zit
        public List<Order> Orders { get; set; } = new();
        
        // Dit zijn alle onderdelen waaruit het product bestaat
        // Bijvoorbeeld: een fiets bestaat uit wielen, een stuur, een zadel, etc.
        public List<Part> Parts { get; set; } = new();
    }
}
