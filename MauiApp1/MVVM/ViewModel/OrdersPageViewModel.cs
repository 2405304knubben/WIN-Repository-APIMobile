using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using MauiApp1.MVVM.Views;
using System.Collections.ObjectModel;

namespace MauiApp1.MVVM.ViewModel
{
    public partial class OrdersPageViewModel : ObservableObject
    {
        private readonly ApiService.ApiService _apiService;
        private List<Order> _allOrders = new();

        public ObservableCollection<Order> Orders { get; } = new();

        // Gebruik partial properties voor WinRT compatibiliteit
        [ObservableProperty]
        private string? statusMessage;

        public OrdersPageViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
            LoadOrdersCommand.Execute(null);

            // Subscribe to navigation events to refresh when returning to this page
            Shell.Current.Navigating += Current_Navigating;
        }

        private void Current_Navigating(object? sender, ShellNavigatingEventArgs e)
        {
            if (e.Current?.Location?.ToString().EndsWith("OrdersPage") == true)
            {
                LoadOrdersCommand.Execute(null);
            }
        }        [RelayCommand]
        public async Task LoadOrders()
        {
            try
            {
                StatusMessage = "Ophalen van orders...";
                var ordersTask = _apiService.GetOrdersAsync();
                var statesTask = _apiService.GetAllDeliveryStatesAsync();

                await Task.WhenAll(ordersTask, statesTask);
                
                var orders = ordersTask.Result;
                var allStates = statesTask.Result;

                // Group states by order ID for quick lookup
                var statesByOrderId = allStates.GroupBy(s => s.OrderId)
                                             .ToDictionary(g => g.Key, g => g.ToList());

                _allOrders.Clear();
                if (orders != null)
                {
                    foreach (var order in orders)
                    {
                        // Update delivery states from API
                        if (statesByOrderId.TryGetValue(order.Id, out var states))
                        {
                            order.DeliveryStates = states;
                        }
                        else
                        {
                            order.DeliveryStates = new List<DeliveryState>();
                        }
                        _allOrders.Add(order);
                    }
                    ShowTodayOrders();
                    StatusMessage = $"Orders van vandaag opgehaald: {Orders.Count}";
                }

                if (!Orders.Any())
                    StatusMessage = "Geen orders gevonden.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij ophalen orders: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ShowAllOrders()
        {
            Orders.Clear();
            foreach (var order in _allOrders)
            {
                Orders.Add(order);
            }
            StatusMessage = $"Alle orders ({Orders.Count})";
        }

        [RelayCommand]
        private void ShowTodayOrders()
        {
            var today = DateTime.Today;
            Orders.Clear();
            foreach (var order in _allOrders.Where(o => o.OrderDate.Date == today))
            {
                Orders.Add(order);
            }
            StatusMessage = $"Orders van vandaag ({Orders.Count})";
        }

        [RelayCommand]
        private void ShowActiveOrders()
        {
            Orders.Clear();
            foreach (var order in _allOrders.Where(o => 
                o.DeliveryStates == null || 
                !o.DeliveryStates.Any() || 
                o.DeliveryStates.LastOrDefault()?.State != DeliveryStateEnum.Delivered))
            {
                Orders.Add(order);
            }
            StatusMessage = $"Actieve orders ({Orders.Count})";
        }

        [RelayCommand]
        private async Task GoToDeliveryTracking(Order order)
        {
            if (order == null)
                return;

            var parameters = new Dictionary<string, object>
            {
                { "Order", order }
            };

            await Shell.Current.GoToAsync($"DeliveryTrackingPage", parameters);
        }

        [RelayCommand]
        public async Task ShowOrderDetails(Order order)
        {
            if (order == null)
            {
                StatusMessage = "Geen order geselecteerd.";
                return;
            }

            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Order", order }
                };
                await Shell.Current.GoToAsync(nameof(OrderDetailsPage), navigationParameter);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij tonen details: {ex.Message}";
            }
        }
    }
}
