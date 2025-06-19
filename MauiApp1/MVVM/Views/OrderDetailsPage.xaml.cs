using System;
using Microsoft.Maui.Controls;
using MauiApp1.ModelAPI;
using MauiApp1.ApiService;
using System.Threading.Tasks;
using MauiApp1.MVVM.ViewModel;

namespace MauiApp1.MVVM.Views
{
    [QueryProperty(nameof(OrderId), "OrderId")]
    public partial class OrderDetailsPage : ContentPage
    {
        private readonly ApiService.ApiService? _apiService;
        private int orderId;
        public int OrderId
        {
            get => orderId;
            set
            {
                orderId = value;
                LoadOrderDetails();
            }
        }
        public OrderDetailsPage()
        {
            InitializeComponent();
            _apiService = Application.Current?.Handler?.MauiContext?.Services?.GetService<ApiService.ApiService>();
            BindingContext = new OrderDetailsPageViewModel();
        }
        private async void LoadOrderDetails()
        {
            if (_apiService == null)
            {
                return;
            }
            if (OrderId <= 0)
            {
                return;
            }
            // Haal alle orders op uit cache/lijst in plaats van losse API-call
            var orders = await _apiService.GetOrdersAsync();
            var found = orders?.Find(o => o.Id == OrderId);
            if (found != null)
            {
                if (BindingContext is OrderDetailsPageViewModel vm)
                {
                    vm.Order = found;
                }
            }
        }
    }
}
