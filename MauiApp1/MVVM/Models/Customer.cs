// Dit bestand beschrijft wat een klant (Customer) is
// Een klant is iemand die spullen bij ons bestelt
using System.Text.Json.Serialization;

namespace MauiApp1.ModelAPI
{
    // Een Customer is een klant die bestellingen kan doen
    public class Customer
    {
        // Elke klant heeft een eigen nummer (Id)
        public int Id { get; set; }
        
        // Dit is de naam van de klant
        public string Name { get; set; } = "";
        
        // Dit is waar de klant woont
        public string Address { get; set; } = "";
        
        // Dit zegt of de klant nog steeds bij ons bestelt (actief is)
        public bool Active { get; set; }

        // Dit is de lijst met alle bestellingen van deze klant
        [JsonIgnore]
        public List<Order> Orders { get; set; } = new();

        // Dit is weer speciaal voor het opslaan van de gegevens
        // Net als een adresboekje waarin je alle informatie netjes opschrijft
        [JsonPropertyName("orders")]
        public List<Order>? OrdersJson
        {
            get => Orders;
            set => Orders = value?.Where(o => o != null).ToList() ?? new();
        }
    }
}
