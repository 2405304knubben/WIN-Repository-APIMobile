using System;
using Microsoft.Maui.Controls;
using MauiApp1.ModelAPI;
using MauiApp1.ApiService;
using System.Threading.Tasks;

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
        }

        private async void LoadOrderDetails()
        {
            if (_apiService == null)
            {
                DebugLabel.Text = "ApiService is null";
                return;
            }
            if (OrderId <= 0)
            {
                DebugLabel.Text = $"OrderId is ongeldig: {OrderId}";
                return;
            }
            var orders = await _apiService.GetOrdersAsync();
            DebugLabel.Text = $"Orders count: {orders?.Count ?? 0}, OrderId: {OrderId}";
            var found = orders?.Find(o => o.Id == OrderId);
            if (found != null)
            {
                BindingContext = found;
                DebugLabel.Text += $"\nOrder gevonden: {found.Id}, Klant: {found.Customer?.Name}";
            }
            else
            {
                DebugLabel.Text += "\nOrder niet gevonden.";
            }
        }
    }
}
