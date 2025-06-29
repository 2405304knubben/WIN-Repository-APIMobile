// Dit bestand beschrijft een statusupdate van een bezorging
// Het is zoals een pakketje volgen: "Het pakje is onderweg!", "Het pakje is bezorgd!"
using System.Text.Json.Serialization;

namespace MauiApp1.ModelAPI
{
    // DeliveryState houdt bij wat er met een bezorging gebeurt
    public partial class DeliveryState
    {
        // Elke statusupdate heeft een eigen nummer
        public int Id { get; set; }
        
        // Dit is de status zelf (In Afwachting, Onderweg, Bezorgd, etc.)
        public DeliveryStateEnum State { get; set; }
        
        // Dit is wanneer deze status is toegevoegd
        public DateTime DateTime { get; set; }
        
        // Dit is het nummer van de bestelling waar deze status bij hoort
        public int OrderId { get; set; }
        
        // Dit is de hele bestelling waar deze status bij hoort
        [JsonIgnore]
        public Order? Order { get; set; }
        
        // Dit is het nummer van de bezorgdienst (zoals PostNL of DHL)
        public int DeliveryServiceId { get; set; }
        
        // Dit is alle informatie over de bezorgdienst
        [JsonIgnore]
        public DeliveryService? DeliveryService { get; set; }
        
        // Dit maakt van de status een mooi leesbaar berichtje
        // Bijvoorbeeld: "Jouw pakketje is onderweg!"
        public string StateDisplay => State.GetDisplayText();
    }
}
