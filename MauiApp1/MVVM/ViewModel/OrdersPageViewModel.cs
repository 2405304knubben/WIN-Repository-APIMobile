using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using MauiApp1.MVVM.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiApp1.MVVM.ViewModel
{
    public partial class OrdersPageViewModel : ObservableObject
    {
        private readonly ApiService.ApiService _apiService; // <-- Volledige naam

        public ObservableCollection<Order> Orders { get; } = new();

        [ObservableProperty]
        string statusMessage;

        [ObservableProperty]
        Order? selectedOrder;

        public OrdersPageViewModel(ApiService.ApiService apiService) // <-- Volledige naam
        {
            _apiService = apiService;
            LoadOrdersCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadOrders()
        {
            try
            {
                StatusMessage = "Ophalen van orders...";
                var orders = await _apiService.GetOrdersAsync();
                StatusMessage = $"Aantal opgehaalde orders: {orders?.Count ?? 0}";

                Orders.Clear();
                foreach (var order in orders)
                    Orders.Add(order);

                if (orders == null || orders.Count == 0)
                    StatusMessage = "Geen orders ontvangen van de API.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij ophalen orders: {ex.Message}";
            }
        }

        

        [RelayCommand]
        public async Task ShowOrderDetails(Order order)
        {
            // Open een nieuwe pagina met details
            await Shell.Current.GoToAsync(nameof(OrderDetailsPage), new Dictionary<string, object>
            {
                ["Order"] = order
            });
        }

        [RelayCommand]
        public async Task CompleteOrder(Order order)
        {
            // Roep je ApiService aan om de order af te ronden
            await _apiService.CompleteDeliveryAsync(order.Id);
            await LoadOrders(); // Refresh de lijst
        }
    }
}
