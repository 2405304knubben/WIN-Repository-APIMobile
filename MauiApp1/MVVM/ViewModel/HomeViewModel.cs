using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using CommunityToolkit.Mvvm.Input;

namespace MauiApp1.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        string welcomeMessage = "Welkom op de HomePage!";

        [ObservableProperty]
        string statusMessage;

        public ObservableCollection<DeliveryState> DeliveryStates { get; } = new();
        public ObservableCollection<Order> Orders { get; } = new();

        private readonly ApiService.ApiService _apiService;

        public HomeViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
            // LoadOrders(); // Functie werkt, maar is nu niet nodig. Later te activeren.
        }

        [RelayCommand]
        public async Task ShowAddOrder()
        {
            await Shell.Current.GoToAsync("///AddOrderPage");
        }

        [RelayCommand]
        public async Task ShowOrders()
        {
            await Shell.Current.GoToAsync("///OrdersPage");
        }

        /* 
         * De onderstaande functies werken, maar zijn nu niet nodig.
         * Je kunt ze later eenvoudig activeren door de comments te verwijderen.
         */

        /*
        private async void LoadDeliveryStates()
        {
            try
            {
                StatusMessage = "Ophalen van DeliveryStates...";
                var states = await _apiService.GetAllDeliveryStatesAsync();
                StatusMessage = $"Aantal opgehaalde DeliveryStates: {states?.Count ?? 0}";

                DeliveryStates.Clear();
                foreach (var state in states)
                    DeliveryStates.Add(state);

                if (states == null || states.Count == 0)
                    StatusMessage = "Geen DeliveryStates ontvangen van de API.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij ophalen DeliveryStates: {ex.Message}";
            }
        }

        private async void LoadOrders()
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

        [ObservableProperty]
        int newOrderCustomerId;

        [ObservableProperty]
        string newOrderStatus;

        [RelayCommand]
        public async Task PostOrder()
        {
            var order = new Order
            {
                CustomerId = NewOrderCustomerId,
                OrderDate = DateTime.Now,
                Products = new List<Product>(),
                DeliveryStates = new List<DeliveryState>()
            };

            var result = await _apiService.PostOrderAsync(order);
            if (result)
            {
                NewOrderStatus = "Order succesvol aangemaakt!";
                LoadOrders();
            }
            else
            {
                NewOrderStatus = "Fout bij aanmaken order.";
            }
        }
        */
    }
}
