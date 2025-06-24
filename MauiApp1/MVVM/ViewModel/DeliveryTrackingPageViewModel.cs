using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using System.Threading.Tasks;

namespace MauiApp1.MVVM.ViewModel
{
    [QueryProperty(nameof(Order), "Order")]
    public partial class DeliveryTrackingPageViewModel : ObservableObject
    {
        private readonly ApiService.ApiService _apiService;

        [ObservableProperty]
        private Order? order;

        [ObservableProperty]
        private string? statusMessage;

        [ObservableProperty]
        private string? mapImageUrl;

        [ObservableProperty]
        private string? mapStatusMessage;

        public DeliveryState? LastDeliveryState => Order?.DeliveryStates?.OrderByDescending(s => s.DateTime).FirstOrDefault();
        public bool ShowStartButton => LastDeliveryState?.State == DeliveryStateEnum.Pending || LastDeliveryState == null;
        public bool ShowCompleteButton => LastDeliveryState?.State == DeliveryStateEnum.InTransit;

        public DeliveryTrackingPageViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        private async Task StartDelivery()
        {
            if (Order == null) return;

            try
            {
                StatusMessage = "Bezorging wordt gestart...";
                await _apiService.StartDeliveryAsync(Order.Id);
                Order = await _apiService.GetOrderByIdAsync(Order.Id); // Refresh order data
                UpdateStatus();
                UpdateProperties();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij starten bezorging: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task CompleteDelivery()
        {
            if (Order == null) return;

            try
            {
                StatusMessage = "Bezorging wordt afgerond...";
                await _apiService.CompleteDeliveryAsync(Order.Id);
                Order = await _apiService.GetOrderByIdAsync(Order.Id); // Refresh order data
                UpdateStatus();
                UpdateProperties();

                await Task.Delay(500);
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij afronden bezorging: {ex.Message}";
            }
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(LastDeliveryState));
            OnPropertyChanged(nameof(ShowStartButton));
            OnPropertyChanged(nameof(ShowCompleteButton));
        }

        private string GetStatusMessage(DeliveryStateEnum? state)
        {
            return state switch
            {
                DeliveryStateEnum.Pending => "Klik op 'Start bezorging' om te beginnen",
                DeliveryStateEnum.InTransit => "Bezorging is onderweg. Klik op 'Rond bezorging af' wanneer afgeleverd.",
                DeliveryStateEnum.Delivered => "Bezorging is afgerond",
                DeliveryStateEnum.Cancelled => "Bezorging is geannuleerd",
                _ => "Status onbekend"
            };
        }

        public void UpdateStatus()
        {
            StatusMessage = GetStatusMessage(LastDeliveryState?.State);
        }

        partial void OnOrderChanged(Order? value)
        {
            UpdateProperties();
            UpdateStatus();

            // Mapbox static map logic
            if (value?.Customer?.Address is not null && !string.IsNullOrWhiteSpace(value.Customer.Address))
            {
                // Amsterdam Centraal
                double lat = 52.379189;
                double lon = 4.899431;
                MapImageUrl = string.Format(
                    "https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-s-l+000({0},{1})/{0},{1},14,0/600x300?access_token=pk.eyJ1IjoibmllbHNjcmVtZXJzIiwiYSI6ImNtNHJsdjNxZzA2cWoya3BkcXp0M2l3N3EifQ.7g4Ms4TNd9ZJPD0EcDM_yw",
                    lon.ToString(CultureInfo.InvariantCulture),
                    lat.ToString(CultureInfo.InvariantCulture)
                );
                MapStatusMessage = null;
            }
            else
            {
                MapImageUrl = null;
                MapStatusMessage = "Geen adres gevonden";
            }
        }
    }
}
