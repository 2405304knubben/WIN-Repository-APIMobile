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

        // Gebruik partial properties voor WinRT compatibiliteit
        [ObservableProperty]
        private string? statusMessage = string.Empty;

        [ObservableProperty]
        private Order? selectedOrder;

        public OrdersPageViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
            statusMessage = string.Empty;
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
                if (orders != null)
                {
                    foreach (var order in orders)
                        Orders.Add(order);
                }

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
            if (order == null)
            {
                StatusMessage = "Geen order geselecteerd.";
                return;
            }
            // Navigeer met expliciete route-naam
            await Shell.Current.GoToAsync($"OrderDetailsPage?OrderId={order.Id}");
        }

        [RelayCommand]
        public async Task CompleteOrder(Order order)
        {
            if (order == null)
            {
                StatusMessage = "Geen order geselecteerd.";
                return;
            }
            try
            {
                await _apiService.CompleteDeliveryAsync(order.Id);
                await LoadOrders(); // Refresh de lijst
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij afronden order: {ex.Message}";
            }
        }
    }
}
