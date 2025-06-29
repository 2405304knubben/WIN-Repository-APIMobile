// Dit bestand zorgt ervoor dat we adressen kunnen openen in een kaart-app
// Het is alsof je op je telefoon een adres aantikt en dan de route erheen kan zien
using System.Web;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

namespace MauiApp1.Services
{
    // Deze helper helpt ons met het openen van adressen in kaart-apps
    public class MapService
    {
        // Dit is de standaard kaart-app van de telefoon
        private readonly IMap _map;
        // Dit gebruiken we om adressen om te zetten naar coördinaten
        private readonly MapboxService _mapboxService;

        // Als we deze helper maken, zorgen we dat we alles hebben wat we nodig hebben
        public MapService(MapboxService mapboxService)
        {
            _map = Map.Default;
            _mapboxService = mapboxService;
        }

        // Deze functie opent een adres in een kaart-app
        // Als useGoogleMaps true is, proberen we Google Maps te gebruiken
        public async Task OpenInMapAsync(string address, bool useGoogleMaps = false)
        {
            try
            {
                // Eerst zoeken we uit waar het adres precies ligt (de coördinaten)
                var coordinates = await _mapboxService.GetCoordinatesAsync(address);
                if (!coordinates.HasValue)
                {
                    // Als we het adres niet kunnen vinden, laten we een foutmelding zien
                    var window = Application.Current?.Windows.FirstOrDefault();
                    if (window?.Page != null)
                    {
                        await window.Page.DisplayAlert("Error", "Could not find coordinates for this address", "OK");
                    }
                    return;
                }

                // We maken de coördinaten en het adres klaar om te gebruiken
                var (latitude, longitude) = coordinates.Value;
                var encodedCoords = $"{latitude},{longitude}";
                var encodedAddress = HttpUtility.UrlEncode(address);

                // Als we Google Maps willen gebruiken...
                if (useGoogleMaps)
                {   
                    // En we zijn op een Android telefoon...
                    if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        try
                        {
                            // Dan kijken we eerst of Google Maps geïnstalleerd is
                            if (await Launcher.CanOpenAsync("com.google.android.apps.maps"))
                            {
                                // Als dat zo is, openen we het adres in Google Maps
                                var geoUri = $"geo:{encodedCoords}?q={encodedCoords}({encodedAddress})";
                                await Launcher.OpenAsync(geoUri);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to open Google Maps app: {ex.Message}");
                            // Ga verder met de fallback naar de website
                        }
                    }
                    
                    // Als Google Maps app niet beschikbaar is, of we zijn niet op Android, 
                    // vallen we terug op de Google Maps website met de coördinaten
                    var googleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={encodedCoords}";
                    await Browser.OpenAsync(googleMapsUrl, BrowserLaunchMode.SystemPreferred);
                }
                else
                {
                    // Als we geen Google Maps willen gebruiken, gebruiken we de ingebouwde kaartfunctionaliteit met de echte coördinaten
                    var location = new Location(latitude, longitude);
                    await _map.OpenAsync(location, new MapLaunchOptions 
                    { 
                        Name = address,
                        NavigationMode = NavigationMode.None
                    });
                }
            }
            catch (Exception ex)
            {
                // Log of behandel de fout op een geschikte manier
                System.Diagnostics.Debug.WriteLine($"Error opening map: {ex.Message}");
            }
        }
    }
}
