// Dit bestand beschrijft wat een bestelling (Order) is
// Een bestelling is zoals een boodschappenlijstje met spullen die iemand wil hebben
using System.Text.Json.Serialization;

namespace MauiApp1.ModelAPI
{
    // Een Order is een bestelling van een klant
    public class Order
    {
        // Elk bestelling heeft een eigen nummer (Id)
        public int Id { get; set; }
        
        // Dit is wanneer de bestelling is gedaan
        public DateTime OrderDate { get; set; }
        
        // Dit is het nummer van de klant die de bestelling heeft gedaan
        public int CustomerId { get; set; }
        
        // Dit is alle informatie over de klant
        public Customer Customer { get; set; } = default!;

        // Dit is de lijst met alle producten in de bestelling
        private List<Product> _products = new();
        [JsonIgnore]
        public List<Product> Products 
        { 
            // We geven alleen de producten terug die echt bestaan (niet null zijn)
            get => _products.Where(p => p != null).ToList();
            set => _products = value?.Where(p => p != null).ToList() ?? new();
        }

        // Dit is de lijst met alle statusupdates van de bezorging
        // Bijvoorbeeld: "In afwachting", "Onderweg", "Bezorgd"
        private List<DeliveryState> _deliveryStates = new();
        [JsonIgnore]
        public List<DeliveryState> DeliveryStates
        {
            // We sorteren de statussen op tijd, zodat de nieuwste bovenaan staat
            get => _deliveryStates.Where(d => d != null).OrderByDescending(d => d.DateTime).ToList();
            set => _deliveryStates = value?.Where(d => d != null).ToList() ?? new();
        }

        // Deze twee properties hieronder zijn speciaal voor het opslaan van de gegevens
        // Het is net als een verjaardagslijstje dat je netjes moet opschrijven
        [JsonPropertyName("products")]
        public List<Product>? ProductsJson
        {
            get => _products;
            set => Products = value ?? new();
        }

        [JsonPropertyName("deliveryStates")]
        public List<DeliveryState>? DeliveryStatesJson
        {
            get => _deliveryStates;
            set => DeliveryStates = value ?? new();
        }
    }
}
