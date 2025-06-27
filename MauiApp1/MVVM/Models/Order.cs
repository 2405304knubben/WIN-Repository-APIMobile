using System.Text.Json.Serialization;

namespace MauiApp1.ModelAPI
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;

        private List<Product> _products = new();
        [JsonIgnore]
        public List<Product> Products 
        { 
            get => _products.Where(p => p != null).ToList();
            set => _products = value?.Where(p => p != null).ToList() ?? new();
        }

        private List<DeliveryState> _deliveryStates = new();
        [JsonIgnore]
        public List<DeliveryState> DeliveryStates
        {
            get => _deliveryStates.Where(d => d != null).OrderByDescending(d => d.DateTime).ToList();
            set => _deliveryStates = value?.Where(d => d != null).ToList() ?? new();
        }

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
