using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using MauiApp1.MVVM.Views;
using System.Collections.ObjectModel;

namespace MauiApp1.MVVM.ViewModel
{
    public enum OrdersFilter
    {
        All,
        Pending,
        InTransit,
        Delivered
    }

    public partial class OrdersPageViewModel : ObservableObject
    {
        private readonly ApiService.ApiService _apiService;
        private List<Order> _allOrders = new();

        public ObservableCollection<Order> Orders { get; } = new();

        [ObservableProperty]
        private string? statusMessage;

        [ObservableProperty]
        private OrdersFilter currentFilter = OrdersFilter.All;

        [ObservableProperty]
        private string? searchText;

        [ObservableProperty]
        private bool onlyToday;

        public double AllOrdersButtonOpacity => CurrentFilter == OrdersFilter.All ? 1.0 : 0.5;
        public double PendingButtonOpacity => CurrentFilter == OrdersFilter.Pending ? 1.0 : 0.5;
        public double InTransitButtonOpacity => CurrentFilter == OrdersFilter.InTransit ? 1.0 : 0.5;
        public double DeliveredButtonOpacity => CurrentFilter == OrdersFilter.Delivered ? 1.0 : 0.5;

        public OrdersPageViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
            LoadOrdersCommand.Execute(null);
            Shell.Current.Navigating += Current_Navigating;
        }

        private void Current_Navigating(object? sender, ShellNavigatingEventArgs e)
        {
            if (e.Current?.Location?.ToString().EndsWith("OrdersPage") == true)
            {
                LoadOrdersCommand.Execute(null);
            }
        }

        [RelayCommand]
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

                var statesByOrderId = allStates.GroupBy(s => s.OrderId)
                                               .ToDictionary(g => g.Key, g => g.ToList());

                _allOrders.Clear();
                if (orders != null)
                {
                    foreach (var order in orders)
                    {
                        if (statesByOrderId.TryGetValue(order.Id, out var states))
                            order.DeliveryStates = states;
                        else
                            order.DeliveryStates = new List<DeliveryState>();
                        _allOrders.Add(order);
                    }
                    ApplyFilter();
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
            CurrentFilter = OrdersFilter.All;
            ApplyFilter();
        }

        [RelayCommand]
        private void ShowPendingOrders()
        {
            CurrentFilter = OrdersFilter.Pending;
            ApplyFilter();
        }

        [RelayCommand]
        private void ShowInTransitOrders()
        {
            CurrentFilter = OrdersFilter.InTransit;
            ApplyFilter();
        }

        [RelayCommand]
        private void ShowDeliveredOrders()
        {
            CurrentFilter = OrdersFilter.Delivered;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Orders.Clear();
            IEnumerable<Order> filtered = _allOrders;

            // Filter op status
            switch (CurrentFilter)
            {
                case OrdersFilter.Pending:
                    filtered = filtered.Where(o =>
                        o.DeliveryStates == null ||
                        !o.DeliveryStates.Any() ||
                        o.DeliveryStates.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State == DeliveryStateEnum.Pending
                    );
                    StatusMessage = $"Niet gestarte bestellingen ({filtered.Count()})";
                    break;
                case OrdersFilter.InTransit:
                    filtered = filtered.Where(o =>
                        o.DeliveryStates != null &&
                        o.DeliveryStates.Any() &&
                        o.DeliveryStates.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State == DeliveryStateEnum.InTransit
                    );
                    StatusMessage = $"Onderweg ({filtered.Count()})";
                    break;
                case OrdersFilter.Delivered:
                    filtered = filtered.Where(o =>
                        o.DeliveryStates != null &&
                        o.DeliveryStates.Any() &&
                        o.DeliveryStates.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State == DeliveryStateEnum.Delivered
                    );
                    StatusMessage = $"Bezorgd ({filtered.Count()})";
                    break;
                case OrdersFilter.All:
                default:
                    StatusMessage = $"Alle bestellingen ({filtered.Count()})";
                    break;
            }

            // Filter op vandaag als de checkbox aan staat
            if (OnlyToday)
            {
                var today = DateTime.Today;
                filtered = filtered.Where(o => o.OrderDate.Date == today);
                StatusMessage += " (vandaag)";
            }

            // Filter op zoektekst
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var lower = SearchText.ToLower();
                filtered = filtered.Where(o =>
                    o.Id.ToString() == lower ||
                    (o.Customer?.Name?.ToLower().Contains(lower) ?? false)
                );
            }

            foreach (var order in filtered
                .OrderBy(o => GetLastState(o) == DeliveryStateEnum.Delivered)
                .ThenByDescending(o => o.OrderDate))
            {
                Orders.Add(order);
            }
        }

        partial void OnOnlyTodayChanged(bool value)
        {
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string? value)
        {
            ApplyFilter();
        }

        private DeliveryStateEnum? GetLastState(Order order)
        {
            return order.DeliveryStates?.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State;
        }

        partial void OnCurrentFilterChanged(OrdersFilter value)
        {
            OnPropertyChanged(nameof(AllOrdersButtonOpacity));
            OnPropertyChanged(nameof(PendingButtonOpacity));
            OnPropertyChanged(nameof(InTransitButtonOpacity));
            OnPropertyChanged(nameof(DeliveredButtonOpacity));
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
    }
}
