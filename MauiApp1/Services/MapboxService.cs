// Dit bestand helpt ons om adressen om te zetten naar coördinaten (waar het precies op de kaart ligt)
// Het is als een heel slim adresboek dat precies weet waar elk adres is
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
    // Deze helper praat met Mapbox (een kaartendienst) om adressen te vinden
    public class MapboxService
    {
        // Dit gebruiken we om met het internet te praten
        private readonly HttpClient _httpClient;
        // Dit is onze speciale sleutel om Mapbox te mogen gebruiken
        private readonly string _apiKey;
        // Dit is een lijst met adressen die we al kennen
        // Het is als een klein adresboekje in ons geheugen
        private readonly Dictionary<string, (double Latitude, double Longitude)> _knownAddresses;

        // Als we deze helper maken, hebben we een sleutel nodig voor Mapbox
        public MapboxService(string apiKey)
        {
            _apiKey = apiKey;
            // We maken een nieuwe internetverbinding klaar
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.mapbox.com")
            };
            
            // We maken een lijst met adressen die we al kennen
            // Dit zijn voorbeeldadressen met hun exacte locatie op de kaart
            _knownAddresses = new Dictionary<string, (double, double)>(StringComparer.OrdinalIgnoreCase)
            {
                // Het eerste getal is hoe ver naar boven/beneden op de kaart (latitude)
                // Het tweede getal is hoe ver naar links/rechts op de kaart (longitude)
                { "123 Elm St", (36.039581, -114.981758) },  // Dit is in Henderson, NV
                { "456 Oak St", (34.697552, -79.910667) }    // Dit is in Cheraw, SC
            };
        }

        // Deze functie zoekt uit waar een adres precies ligt
        public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string address)
        {
            Debug.WriteLine($"[Mapbox] Looking up address: {address}");

            // Eerst kijken we of het een adres is dat we al kennen
            // We pakken alleen het straatgedeelte (bijvoorbeeld "123 Elm St" van "123 Elm St, New York")
            var simpleAddress = address.Split(',')[0].Trim();
            Debug.WriteLine($"[Mapbox] Simplified address: {simpleAddress}");

            // Als we het adres al kennen, geven we meteen de locatie terug
            if (_knownAddresses.TryGetValue(simpleAddress, out var coordinates))
            {
                Debug.WriteLine($"[Mapbox] Found known address! Coordinates: {coordinates.Latitude}, {coordinates.Longitude}");
                return coordinates;
            }

            // Als we het adres nog niet kennen, vragen we het aan Mapbox
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
