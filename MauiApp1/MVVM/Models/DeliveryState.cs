using System.Text.Json.Serialization;

namespace MauiApp1.ModelAPI
{
    public partial class DeliveryState
    {
        public int Id { get; set; }
        public DeliveryStateEnum State { get; set; }
        public DateTime DateTime { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order? Order { get; set; }
        public int DeliveryServiceId { get; set; }
        [JsonIgnore]
        public DeliveryService? DeliveryService { get; set; }
        public string StateDisplay => State.GetDisplayText();
    }
}
