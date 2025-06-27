using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using MauiApp1.Services;
using System.Threading.Tasks;

namespace MauiApp1.MVVM.ViewModel
{
    [QueryProperty(nameof(Order), "Order")]
    public partial class DeliveryTrackingPageViewModel : ObservableObject, IDisposable
    {
        private readonly ApiService.ApiService _apiService;
        private readonly MapboxService _mapboxService;
        private readonly string _mapboxApiKey;
        private readonly MapService _mapService;
        private bool _useGoogleMaps;
        private IDispatcherTimer? _refreshTimer;
        private bool _isDisposed;

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

        // Overlay alleen tonen als de order onderweg is
        public bool ShowMapOverlay => LastDeliveryState?.State == DeliveryStateEnum.InTransit;

        public DeliveryTrackingPageViewModel(ApiService.ApiService apiService, Services.MapboxService mapboxService, string mapboxApiKey, MapService mapService)
        {
            _apiService = apiService;
            _mapboxService = mapboxService;
            _mapboxApiKey = mapboxApiKey;
            _mapService = mapService;

            // Create and start the refresh timer
            _refreshTimer = Application.Current!.Dispatcher.CreateTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(5);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            if (Order != null)
            {
                var updatedOrder = await _apiService.GetOrderByIdAsync(Order.Id);
                if (updatedOrder != null &&
                    (Order.DeliveryStates == null ||
                     updatedOrder.DeliveryStates?.Count != Order.DeliveryStates?.Count ||
                     updatedOrder.DeliveryStates?.LastOrDefault()?.State != Order.DeliveryStates?.LastOrDefault()?.State))
                {
                    Order = updatedOrder;
                }
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            _refreshTimer?.Stop();
            var navigationParameter = new Dictionary<string, object>
            {
                { "RefreshRequired", true }
            };
            await Shell.Current.GoToAsync("..", navigationParameter);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _refreshTimer?.Stop();
                    _refreshTimer = null;
                }
                _isDisposed = true;
            }
        }

        [RelayCommand]
        private async Task StartDelivery()
        {
            if (Order == null) return;

            try
            {
                StatusMessage = "Bezorging wordt gestart...";
                await _apiService.StartDeliveryAsync(Order.Id);
                Order = await _apiService.GetOrderByIdAsync(Order.Id);
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

#if ANDROID || IOS
                    // Vibrate three times with high intensity
                    for (int i = 0; i < 3; i++)
                    {
                        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(1000));
                        await Task.Delay(200);
                    }
#endif
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij afronden bezorging: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task OpenMap()
        {
            if (Order?.Customer?.Address == null) return;

            try
            {
                await _mapService.OpenInMapAsync(Order.Customer.Address, _useGoogleMaps);
                _useGoogleMaps = !_useGoogleMaps; // Toggle for next click
            }
            catch (Exception ex)
            {
                StatusMessage = $"Er is een fout opgetreden bij het openen van de kaart: {ex.Message}";
            }
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(LastDeliveryState));
            OnPropertyChanged(nameof(ShowStartButton));
            OnPropertyChanged(nameof(ShowCompleteButton));
            OnPropertyChanged(nameof(ShowMapOverlay)); // <-- belangrijk!
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

        private void LoadMap()
        {
            if (Order?.Customer?.Address is not null && !string.IsNullOrWhiteSpace(Order.Customer.Address))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"[Map] Loading map for address: {Order.Customer.Address}");
                    MapStatusMessage = "Kaart laden...";
                    MapImageUrl = null; // Clear previous image

                    // Get the street part of the address
                    var streetAddress = Order.Customer.Address.Split(',')[0].Trim();

                    // Hardcoded coordinates for the known addresses
                    switch (streetAddress.ToLower())
                    {
                        case "123 elm st":
                            // Henderson, NV coordinates
                            MapImageUrl = string.Format(
                                CultureInfo.InvariantCulture,
                                "https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-s-l+000({0},{1})/{0},{1},14,0/600x300?access_token={2}&t={3}",
                                "-114.981758", // longitude
                                "36.039581",   // latitude
                                _mapboxApiKey,
                                DateTime.UtcNow.Ticks
                            );
                            MapStatusMessage = null;
                            break;

                        case "456 oak st":
                            // Cheraw, SC coordinates
                            MapImageUrl = string.Format(
                                CultureInfo.InvariantCulture,
                                "https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-s-l+000({0},{1})/{0},{1},14,0/600x300?access_token={2}&t={3}",
                                "-79.910667",  // longitude
                                "34.697552",   // latitude
                                _mapboxApiKey,
                                DateTime.UtcNow.Ticks
                            );
                            MapStatusMessage = null;
                            break;

                        default:
                            MapStatusMessage = "Adres kon niet worden gevonden op de kaart.";
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine($"[Map] Generated map URL: {MapImageUrl}");
                    OnPropertyChanged(nameof(MapImageUrl));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Map] Error: {ex.Message}");
                    MapStatusMessage = "Fout bij het ophalen van de kaartlocatie.";
                }
            }
            else
            {
                MapImageUrl = null;
                MapStatusMessage = "Geen adres beschikbaar om op de kaart te tonen.";
            }
        }

        partial void OnOrderChanged(Order? value)
        {
            UpdateProperties();
            UpdateStatus();
            LoadMap();
        }
    }
}
