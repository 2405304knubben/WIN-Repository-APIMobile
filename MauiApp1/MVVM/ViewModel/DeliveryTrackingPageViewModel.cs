using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using Microsoft.Maui.ApplicationModel;


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

        public DeliveryState? LastDeliveryState => Order?.DeliveryStates?.OrderByDescending(s => s.DateTime).FirstOrDefault();        public bool ShowStartButton => LastDeliveryState?.State == DeliveryStateEnum.Pending || LastDeliveryState == null;
        public bool ShowCompleteButton => LastDeliveryState?.State == DeliveryStateEnum.InTransit;        private string GetStatusMessage(DeliveryStateEnum? state)
        {
            return state switch
            {
                DeliveryStateEnum.Pending => "Klik op 'Start bezorging' om te beginnen",
                DeliveryStateEnum.InTransit => "Bezorging is onderweg. Klik op 'Rond bezorging af' wanneer afgeleverd.",
                DeliveryStateEnum.Delivered => "Bezorging is afgerond",
                _ => "Status onbekend"
            };
        }

        public void UpdateStatus()
        {
            StatusMessage = GetStatusMessage(LastDeliveryState?.State);
        }

        public DeliveryTrackingPageViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
        }        [RelayCommand]
        private async Task StartDelivery()
        {
            if (Order == null) return;

            try
            {
                StatusMessage = "Bezorging wordt gestart...";
                var result = await _apiService.StartDeliveryAsync(Order.Id);
                
                if (result?.Any() == true)
                {
                    // Update alleen de delivery states, niet de hele order
                    Order.DeliveryStates = result;
                    UpdateStatus();
                    UpdateProperties();
                }
                else
                {
                    StatusMessage = "Fout bij starten: Geen status ontvangen";
                }
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
                Order = await _apiService.GetOrderByIdAsync(Order.Id);
                UpdateStatus();
                UpdateProperties();

                // Wacht 0,5 seconde en trillen
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

        partial void OnOrderChanged(Order? value)
        {
            UpdateProperties();
        }
    }
}
