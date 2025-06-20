using System.Text.Json.Serialization;

namespace MauiApp1.ModelAPI
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public bool Active { get; set; }

        [JsonIgnore]
        public List<Order> Orders { get; set; } = new();

        [JsonPropertyName("orders")]
        public List<Order>? OrdersJson
        {
            get => Orders;
            set => Orders = value?.Where(o => o != null).ToList() ?? new();
        }
    }
}
