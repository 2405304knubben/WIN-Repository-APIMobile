using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using MauiApp1.Services;
using System.Threading.Tasks;

namespace MauiApp1.MVVM.ViewModel
{
    // Dit helpt de pagina om informatie over pakketbezorging te laten zien
    [QueryProperty(nameof(Order), "Order")]
    public partial class DeliveryTrackingPageViewModel : ObservableObject, IDisposable
    {
        // Deze helpers helpen ons om informatie op te halen en kaarten te laten zien
        private readonly ApiService.ApiService _apiService;
        private readonly MapboxService _mapboxService;
        private readonly string _mapboxApiKey;
        private readonly MapService _mapService;
        private bool _useGoogleMaps;
        // Dit is een klokje dat elke paar seconden kijkt of er nieuwe informatie is
        private IDispatcherTimer? _refreshTimer;
        private bool _isDisposed;

        // De bestelling waar we naar kijken
        [ObservableProperty]
        private Order? order;

        // Een bericht dat vertelt wat er gebeurt, zoals "Je pakje is onderweg!"
        [ObservableProperty]
        private string? statusMessage;

        // Een plaatje van de kaart die laat zien waar het pakje is
        [ObservableProperty]
        private string? mapImageUrl;

        // Een bericht over de kaart
        [ObservableProperty]
        private string? mapStatusMessage;

        // Dit geeft ons de nieuwste informatie over waar het pakje is
        public DeliveryState? LastDeliveryState => Order?.DeliveryStates?.OrderByDescending(s => s.DateTime).FirstOrDefault();
        // Dit zegt of we een knop moeten laten zien om de bezorging te starten
        public bool ShowStartButton => LastDeliveryState?.State == DeliveryStateEnum.Pending || LastDeliveryState == null;
        // Dit zegt of we een knop moeten laten zien om de bezorging af te maken
        public bool ShowCompleteButton => LastDeliveryState?.State == DeliveryStateEnum.InTransit;

        // We laten alleen extra informatie zien als het pakje onderweg is
        public bool ShowMapOverlay => LastDeliveryState?.State == DeliveryStateEnum.InTransit;

        // Hier maken we alles klaar om te werken
        public DeliveryTrackingPageViewModel(ApiService.ApiService apiService, Services.MapboxService mapboxService, string mapboxApiKey, MapService mapService)
        {
            // We bewaren alle helpers die we nodig hebben
            _apiService = apiService;
            _mapboxService = mapboxService;
            _mapboxApiKey = mapboxApiKey;
            _mapService = mapService;

            // We maken een klokje dat elke 5 seconden checkt of er nieuwe informatie is
            _refreshTimer = Application.Current!.Dispatcher.CreateTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(5);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        // Dit gebeurt elke keer als het klokje tikt
        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            if (Order != null)
            {
                // We vragen om nieuwe informatie over onze bestelling
                var updatedOrder = await _apiService.GetOrderByIdAsync(Order.Id);
                // Als er iets veranderd is, updaten we onze informatie
                if (updatedOrder != null &&
                    (Order.DeliveryStates == null ||
                     updatedOrder.DeliveryStates?.Count != Order.DeliveryStates?.Count ||
                     updatedOrder.DeliveryStates?.LastOrDefault()?.State != Order.DeliveryStates?.LastOrDefault()?.State))
                {
                    Order = updatedOrder;
                }
            }
        }

        // Dit gebeurt als je op de terugknop drukt
        [RelayCommand]
        private async Task GoBack()
        {
            // We stoppen het klokje omdat we weggaan
            _refreshTimer?.Stop();
            // We gaan terug naar de vorige pagina en vertellen dat we nieuwe informatie willen
            var navigationParameter = new Dictionary<string, object>
            {
                { "RefreshRequired", true }
            };
            await Shell.Current.GoToAsync("..", navigationParameter);
        }

        // Dit ruimt alles op als we klaar zijn
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Hier ruimen we alle spullen op die we hebben gebruikt
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // We stoppen het klokje en ruimen het op
                    _refreshTimer?.Stop();
                    _refreshTimer = null;
                }
                _isDisposed = true;
            }
        }

        // Dit gebeurt als je op de knop "Start bezorging" drukt
        [RelayCommand]
        private async Task StartDelivery()
        {
            // Als er geen bestelling is, doen we niets
            if (Order == null) return;

            try
            {
                // We laten zien dat we bezig zijn
                StatusMessage = "Bezorging wordt gestart...";
                // We vragen de computer om de bezorging te starten
                await _apiService.StartDeliveryAsync(Order.Id);
                // We halen de nieuwste informatie over onze bestelling op
                Order = await _apiService.GetOrderByIdAsync(Order.Id);
                // We updaten wat we laten zien
                UpdateStatus();
                UpdateProperties();
            }
            catch (Exception ex)
            {
                // Als er iets fout gaat, laten we dat zien
                StatusMessage = $"Fout bij starten bezorging: {ex.Message}";
            }
        }

        // Dit gebeurt als je op de knop "Rond bezorging af" drukt
        [RelayCommand]
        private async Task CompleteDelivery()
        {
            // Als er geen bestelling is, doen we niets
            if (Order == null) return;

            try
            {
                // We laten zien dat we bezig zijn
                StatusMessage = "Bezorging wordt afgerond...";
                // We vragen de computer om de bezorging af te ronden
                await _apiService.CompleteDeliveryAsync(Order.Id);
                // We halen de nieuwste informatie over onze bestelling op
                Order = await _apiService.GetOrderByIdAsync(Order.Id);
                // We updaten wat we laten zien
                UpdateStatus();
                UpdateProperties();

#if ANDROID || IOS
                    // Op telefoons laten we de telefoon trillen om te vieren dat we klaar zijn
                    for (int i = 0; i < 3; i++)
                    {
                        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(1000));
                        await Task.Delay(200);
                    }
#endif
            }
            catch (Exception ex)
            {
                // Als er iets fout gaat, laten we dat zien
                StatusMessage = $"Fout bij afronden bezorging: {ex.Message}";
            }
        }

        // Dit gebeurt als je op de kaart drukt om hem in de kaart-app te openen
        [RelayCommand]
        private async Task OpenMap()
        {
            // Als er geen adres is, kunnen we geen kaart openen
            if (Order?.Customer?.Address == null) return;

            try
            {
                // We openen het adres in de kaart-app van de telefoon
                await _mapService.OpenInMapAsync(Order.Customer.Address, _useGoogleMaps);
                // De volgende keer gebruiken we een andere kaart-app (Google of Apple)
                _useGoogleMaps = !_useGoogleMaps;
            }
            catch (Exception ex)
            {
                // Als er iets fout gaat, laten we dat zien
                StatusMessage = $"Er is een fout opgetreden bij het openen van de kaart: {ex.Message}";
            }
        }

        // Deze functie zorgt dat alle informatie op het scherm wordt bijgewerkt
        private void UpdateProperties()
        {
            // We vertellen het scherm dat deze dingen misschien veranderd zijn
            OnPropertyChanged(nameof(LastDeliveryState));
            OnPropertyChanged(nameof(ShowStartButton));
            OnPropertyChanged(nameof(ShowCompleteButton));
            OnPropertyChanged(nameof(ShowMapOverlay)); // Dit is belangrijk voor de kaart!
        }

        // Deze functie maakt een bericht op basis van de status van de bestelling
        private string GetStatusMessage(DeliveryStateEnum? state)
        {
            return state switch
            {
                // Verschillende berichten voor verschillende situaties
                DeliveryStateEnum.Pending => "Klik op 'Start bezorging' om te beginnen",
                DeliveryStateEnum.InTransit => "Bezorging is onderweg. Klik op 'Rond bezorging af' wanneer afgeleverd.",
                DeliveryStateEnum.Delivered => "Bezorging is afgerond",
                DeliveryStateEnum.Cancelled => "Bezorging is geannuleerd",
                _ => "Status onbekend"
            };
        }

        // Deze functie past het statusbericht aan
        public void UpdateStatus()
        {
            StatusMessage = GetStatusMessage(LastDeliveryState?.State);
        }

        // Deze functie haalt een kaartplaatje op om te laten zien waar het adres is
        private void LoadMap()
        {
            // Als er een adres is dat niet leeg is
            if (Order?.Customer?.Address is not null && !string.IsNullOrWhiteSpace(Order.Customer.Address))
            {
                try
                {
                    // We schrijven naar de console wat we doen (voor als we fouten zoeken)
                    System.Diagnostics.Debug.WriteLine($"[Map] Loading map for address: {Order.Customer.Address}");
                    MapStatusMessage = "Kaart laden...";
                    MapImageUrl = null; // We maken het oude plaatje weg

                    // We halen alleen het straatnaam deel uit het adres
                    var streetAddress = Order.Customer.Address.Split(',')[0].Trim();

                    // We hebben vaste coördinaten voor bekende adressen
                    // (In het echt zou je een geocoding service gebruiken)
                    switch (streetAddress.ToLower())
                    {
                        case "123 elm st":
                            // Henderson, NV coördinaten
                            MapImageUrl = string.Format(
                                CultureInfo.InvariantCulture,
                                "https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-s-l+000({0},{1})/{0},{1},14,0/600x300?access_token={2}&t={3}",
                                "-114.981758", // lengtegraad
                                "36.039581",   // breedtegraad
                                _mapboxApiKey,
                                DateTime.UtcNow.Ticks
                            );
                            MapStatusMessage = null;
                            break;

                        case "456 oak st":
                            // Cheraw, SC coördinaten
                            MapImageUrl = string.Format(
                                CultureInfo.InvariantCulture,
                                "https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-s-l+000({0},{1})/{0},{1},14,0/600x300?access_token={2}&t={3}",
                                "-79.910667",  // lengtegraad
                                "34.697552",   // breedtegraad
                                _mapboxApiKey,
                                DateTime.UtcNow.Ticks
                            );
                            MapStatusMessage = null;
                            break;

                        default:
                            // Als we het adres niet kennen
                            MapStatusMessage = "Adres kon niet worden gevonden op de kaart.";
                            break;
                    }

                    // We schrijven naar de console welke kaart-URL we gemaakt hebben
                    System.Diagnostics.Debug.WriteLine($"[Map] Generated map URL: {MapImageUrl}");
                    // We vertellen het scherm dat de kaart-URL veranderd is
                    OnPropertyChanged(nameof(MapImageUrl));
                }
                catch (Exception ex)
                {
                    // Als er iets fout gaat, schrijven we dat naar de console
                    System.Diagnostics.Debug.WriteLine($"[Map] Error: {ex.Message}");
                    MapStatusMessage = "Fout bij het ophalen van de kaartlocatie.";
                }
            }
            else
            {
                // Als er geen adres is
                MapImageUrl = null;
                MapStatusMessage = "Geen adres beschikbaar om op de kaart te tonen.";
            }
        }

        // Deze functie wordt automatisch aangeroepen als de bestelling verandert
        partial void OnOrderChanged(Order? value)
        {
            // We updaten alle informatie op het scherm
            UpdateProperties();
            // We updaten het statusbericht
            UpdateStatus();
            // We laden een nieuwe kaart voor het nieuwe adres
            LoadMap();
        }
    }
}
