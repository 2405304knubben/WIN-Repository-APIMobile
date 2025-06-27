using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace MauiApp1.Services
{
    public class MapboxService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly Dictionary<string, (double Latitude, double Longitude)> _knownAddresses;

        public MapboxService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.mapbox.com")
            };
            
            // Initialize known addresses
            _knownAddresses = new Dictionary<string, (double, double)>(StringComparer.OrdinalIgnoreCase)
            {
                { "123 Elm St", (36.039581, -114.981758) },  // Henderson, NV coordinates
                { "456 Oak St", (34.697552, -79.910667) }    // Cheraw, SC coordinates
            };
        }

        public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string address)
        {
            Debug.WriteLine($"[Mapbox] Looking up address: {address}");

            // Check if this is a known address first
            var simpleAddress = address.Split(',')[0].Trim();  // Take just the street part
            Debug.WriteLine($"[Mapbox] Simplified address: {simpleAddress}");

            if (_knownAddresses.TryGetValue(simpleAddress, out var coordinates))
            {
                Debug.WriteLine($"[Mapbox] Found known address! Coordinates: {coordinates.Latitude}, {coordinates.Longitude}");
                return coordinates;
            }

            // If not a known address, try Mapbox geocoding
            var url = $"/geocoding/v5/mapbox.places/{Uri.EscapeDataString(address)}, United States.json?access_token={_apiKey}&limit=1";
            Debug.WriteLine($"[Mapbox] Calling Mapbox API with URL: {url}");

            try 
            {
                var response = await _httpClient.GetAsync(url);
                Debug.WriteLine($"[Mapbox] API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MapboxGeocodingResponse>();
                    Debug.WriteLine($"[Mapbox] Features found: {result?.Features?.Count ?? 0}");

                    var feature = result?.Features?.FirstOrDefault();
                    if (feature?.Center?.Length == 2)
                    {
                        var lat = feature.Center[1];
                        var lon = feature.Center[0];
                        Debug.WriteLine($"[Mapbox] Found coordinates: {lat}, {lon}");
                        return (lat, lon);
                    }
                    else
                    {
                        Debug.WriteLine("[Mapbox] No valid coordinates in response");
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[Mapbox] API Error: {error}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Mapbox] Exception: {ex.Message}");
            }
            return null;
        }
    }

    public class MapboxGeocodingResponse
    {
        [JsonPropertyName("features")]
        public List<Feature>? Features { get; set; }
    }

    public class Feature
    {
        [JsonPropertyName("center")]
        public double[]? Center { get; set; }
    }
}
