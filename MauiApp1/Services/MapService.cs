using System.Web;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

namespace MauiApp1.Services
{
    public class MapService
    {
        private readonly IMap _map;
        private readonly MapboxService _mapboxService;

        public MapService(MapboxService mapboxService)
        {
            _map = Map.Default;
            _mapboxService = mapboxService;
        }

        public async Task OpenInMapAsync(string address, bool useGoogleMaps = false)
        {
            try
            {
                // First get the coordinates for the address
                var coordinates = await _mapboxService.GetCoordinatesAsync(address);
                if (!coordinates.HasValue)
                {
                    var window = Application.Current?.Windows.FirstOrDefault();
                    if (window?.Page != null)
                    {
                        await window.Page.DisplayAlert("Error", "Could not find coordinates for this address", "OK");
                    }
                    return;
                }

                var (latitude, longitude) = coordinates.Value;
                var encodedCoords = $"{latitude},{longitude}";
                var encodedAddress = HttpUtility.UrlEncode(address);

                if (useGoogleMaps)
                {   
                    if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        try
                        {
                            // First try to open in Google Maps app
                            if (await Launcher.CanOpenAsync("com.google.android.apps.maps"))
                            {
                                // Use the geo: URI scheme with actual coordinates
                                var geoUri = $"geo:{encodedCoords}?q={encodedCoords}({encodedAddress})";
                                await Launcher.OpenAsync(geoUri);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to open Google Maps app: {ex.Message}");
                            // Continue to web fallback
                        }
                    }
                    
                    // Fallback to Google Maps website with coordinates
                    var googleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={encodedCoords}";
                    await Browser.OpenAsync(googleMapsUrl, BrowserLaunchMode.SystemPreferred);
                }
                else
                {
                    // Use built-in map functionality with actual coordinates
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
                // Log or handle the error appropriately
                System.Diagnostics.Debug.WriteLine($"Error opening map: {ex.Message}");
            }
        }
    }
}
