// Dit bestand zorgt ervoor dat we kunnen praten met onze server
// Het is als een telefoon waarmee we de server kunnen bellen om informatie te vragen of door te geven
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;

namespace MauiApp1.ApiService
{
    // Deze helper zorgt voor alle communicatie met onze server
    public class ApiService
    {
        // Dit gebruiken we om met de server te praten via het internet
        private readonly HttpClient _client;
        // Dit zijn speciale instellingen voor hoe we berichten naar de server sturen
        private readonly JsonSerializerOptions _jsonOptions;
        // Als de server te lang niet antwoordt (30 seconden), stoppen we met wachten
        private const int DefaultTimeoutSeconds = 30;

        // Als we deze helper maken, hebben we een sleutel en een adres nodig
        public ApiService(string apiKey, string baseUrl)
        {
            // We controleren eerst of we alle belangrijke informatie hebben
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be empty", nameof(apiKey));
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));

            try
            {
                // We maken een speciale internetverbinding die veilig is
                var handler = new HttpClientHandler
                {
                    // Dit controleert of de server wel echt onze server is
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => 
                    {
                        #if DEBUG
                        // Als we aan het testen zijn, vertrouwen we alle servers
                        return true;
                        #else
                        // Als de app echt wordt gebruikt, controleren we goed of het veilig is
                        return errors == System.Net.Security.SslPolicyErrors.None;
                        #endif
                    }
                };

                // We maken de internetverbinding klaar met al onze instellingen
                _client = new HttpClient(handler)
                {
                    // Dit is het adres van onze server
                    BaseAddress = new Uri(baseUrl.TrimEnd('/')),
                    // Als de server 30 seconden niet antwoordt, stoppen we met wachten
                    Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds)
                };
                
                // We vertellen de server dat we JSON-berichten willen sturen en ontvangen
                // JSON is een speciale taal die computers gebruiken om met elkaar te praten
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                // We sturen onze speciale sleutel mee zodat de server weet dat wij het zijn
                _client.DefaultRequestHeaders.Add("ApiKey", apiKey);

                // Dit zijn de instellingen voor hoe we JSON-berichten maken en lezen
                _jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                    WriteIndented = true
                };

                #if DEBUG
                Console.WriteLine($"ApiService initialized with baseUrl: {baseUrl}");
                #endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing ApiService: {ex.Message}");
                throw;
            }
        }

        // Deze functie vraagt de server om een lijst met bestellingen
        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = new List<Order>();
            // Definieer hier je bereik
            int startId = 1200;
            int endId = 1300;

            // We vragen de server om elke bestelling één voor één
            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    // Dit is het adres dat we de server vragen
                    var requestUrl = $"api/Order/{i}";
                    // We sturen de vraag naar de server
                    var response = await _client.GetAsync(requestUrl);
                    // We lezen het antwoord van de server
                    var content = await response.Content.ReadAsStringAsync();

                    // Als de server niet succesvol was, gaan we naar de volgende bestelling
                    if (!response.IsSuccessStatusCode)
                    {
                        // Optioneel: sla niet-gevonden bestellingen over of log ze
                        Debug.WriteLine($"Order {i} not found or error: {response.StatusCode}");
                        continue;
                    }

                    // We proberen de bestelling te begrijpen die de server heeft teruggestuurd
                    var order = JsonSerializer.Deserialize<Order>(content, _jsonOptions);
                    if (order != null)
                    {
                        // Als we de bestelling konden begrijpen, voegen we deze toe aan onze lijst
                        orders.Add(order);
                    }
                }
                catch (JsonException ex)
                {
                    // Als er een fout is bij het begrijpen van de bestelling, loggen we de fout
                    Debug.WriteLine($"Error deserializing order {i}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Voor andere fouten loggen we ook de fout
                    Debug.WriteLine($"Error getting order {i}: {ex.Message}");
                }
            }

            // We geven de lijst met bestellingen terug die we hebben gevonden
            return orders;
        }

        // Deze functie vraagt de server om alle leveringsstatussen
        public async Task<List<DeliveryState>> GetAllDeliveryStatesAsync()
        {
            try
            {
                // Dit is het adres dat we de server vragen voor alle leveringsstatussen
                var requestUrl = "api/DeliveryStates/GetAllDeliveryStates";
                // We sturen de vraag naar de server
                var response = await _client.GetAsync(requestUrl);
                // We lezen het antwoord van de server
                var content = await response.Content.ReadAsStringAsync();
                
                // We controleren of de server succesvol was
                response.EnsureSuccessStatusCode();

                // We proberen de leveringsstatussen te begrijpen die de server heeft teruggestuurd
                var states = JsonSerializer.Deserialize<List<DeliveryState>>(content, _jsonOptions);
                // We geven de lijst met leveringsstatussen terug, of een lege lijst als er geen zijn
                return states ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                // Als er een fout is, loggen we deze en geven we een foutmelding terug
                Debug.WriteLine($"Error fetching delivery states: {ex.Message}");
                throw;
            }
        }

        // Deze functie vertelt de server om een levering te starten voor een bepaalde bestelling
        public async Task StartDeliveryAsync(int orderId)
        {
            try
            {
                // Dit is het adres dat we de server vragen om de levering te starten
                var requestUrl = $"api/DeliveryStates/StartDelivery?OrderId={orderId}";
                // We sturen een verzoek naar de server om de levering te starten
                var response = await _client.PostAsync(requestUrl, null);

                // Als de server niet succesvol was, loggen we de fout en geven we een foutmelding terug
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"StartDelivery Error Response: {content}");
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
            }
            catch (Exception ex)
            {
                // Als er een fout is, loggen we deze en geven we een foutmelding terug
                System.Diagnostics.Debug.WriteLine($"Error in StartDeliveryAsync: {ex.Message}");
                throw;
            }
        }

        // Deze functie vertelt de server om een levering te voltooien voor een bepaalde bestelling
        public async Task CompleteDeliveryAsync(int orderId)
        {
            try
            {
                // Dit is het adres dat we de server vragen om de levering te voltooien
                var requestUrl = $"api/DeliveryStates/CompleteDelivery?OrderId={orderId}";
                // We sturen een verzoek naar de server om de levering te voltooien
                var response = await _client.PostAsync(requestUrl, null);

                // Als de server niet succesvol was, loggen we de fout en geven we een foutmelding terug
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"CompleteDelivery Error Response: {content}");
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
            }
            catch (Exception ex)
            {
                // Als er een fout is, loggen we deze en geven we een foutmelding terug
                System.Diagnostics.Debug.WriteLine($"Error in CompleteDeliveryAsync: {ex.Message}");
                throw;
            }
        }

        // Deze functie vertelt de server dat we een nieuwe bestelling willen plaatsen
        public async Task<bool> PostOrderAsync(Order order)
        {
            try
            {
                // Dit zijn de instellingen voor hoe we de bestelling naar de server sturen
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                // We zetten de bestelling om in een JSON-bericht
                var json = JsonSerializer.Serialize(order, options);
                // We maken een bericht dat we naar de server sturen
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // We sturen het bericht naar de server om de bestelling te plaatsen
                var response = await _client.PostAsync("api/Order", content);
                // We geven terug of de server succesvol was met het plaatsen van de bestelling
                return response.IsSuccessStatusCode;
            }
            catch
            {
                // Als er een fout is, geven we terug dat het niet gelukt is
                return false;
            }
        }

        // Deze functie vraagt de server om een specifieke bestelling op te halen
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            try
            {
                // Dit is het adres dat we de server vragen voor een specifieke bestelling
                var requestUrl = $"api/Order/{orderId}";
                // We sturen de vraag naar de server
                var response = await _client.GetAsync(requestUrl);
                // We lezen het antwoord van de server
                var content = await response.Content.ReadAsStringAsync();
                // Dit zijn de instellingen voor hoe we de bestelling begrijpen die de server heeft teruggestuurd
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                // Als de server niet succesvol was, geven we een foutmelding terug
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
                // We proberen de bestelling te begrijpen die de server heeft teruggestuurd
                var result = JsonSerializer.Deserialize<Order>(content, options);
                // Als we de bestelling niet konden begrijpen, geven we een foutmelding terug
                if (result == null)
                {
                    throw new JsonException("Failed to deserialize order from API response");
                }
                // We geven de bestelling terug die we van de server hebben gekregen
                return result;
            }
            catch (Exception ex)
            {
                // Als er een fout is, geven we een foutmelding terug
                throw new Exception($"Error getting order by id: {ex.Message}", ex);
            }
        }

        // Deze functie vraagt de server om een levering te beëindigen voor een bepaalde bestelling
        public async Task<List<DeliveryState>> FinishDeliveryAsync(int orderId)
        {
            try
            {
                // Dit is het adres dat we de server vragen om de levering te beëindigen
                var requestUrl = $"api/Order/{orderId}/FinishDelivery";
                // We sturen een verzoek naar de server om de levering te beëindigen
                var response = await _client.PostAsync(requestUrl, null);
                // We lezen het antwoord van de server
                var content = await response.Content.ReadAsStringAsync();

                // Als de server niet succesvol was, geven we een foutmelding terug
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode} - {content}");
                }

                // We proberen de nieuwe leveringsstatussen te begrijpen die de server heeft teruggestuurd
                var result = JsonSerializer.Deserialize<List<DeliveryState>>(content, _jsonOptions);
                // We geven de lijst met nieuwe leveringsstatussen terug, of een lege lijst als er geen zijn
                return result ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                // Als er een fout is, loggen we deze en geven we een foutmelding terug
                Debug.WriteLine($"Error in FinishDeliveryAsync: {ex}");
                throw;
            }
        }
        // Deze functie vraagt de server om de leveringsstatussen voor een bereik van bestellingen
        public async Task<List<DeliveryState>> GetDeliveryStatesByOrderRangeAsync(int startId, int endId)
        {
            var allStates = new List<DeliveryState>();
            // Voor elke bestelling in het opgegeven bereik
            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    // Dit is het adres dat we de server vragen voor de leveringsstatus van een specifieke bestelling
                    var requestUrl = $"api/DeliveryStates/GetByOrderId/{i}";
                    // We sturen de vraag naar de server
                    var response = await _client.GetAsync(requestUrl);
                    // We lezen het antwoord van de server
                    var content = await response.Content.ReadAsStringAsync();

                    // Als de server niet succesvol was, loggen we de fout en gaan we verder met de volgende bestelling
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Delivery states for order {i} not found or error: {response.StatusCode}");
                        continue;
                    }

                    // We proberen de leveringsstatussen te begrijpen die de server heeft teruggestuurd
                    var states = JsonSerializer.Deserialize<List<DeliveryState>>(content, _jsonOptions);
                    if (states != null)
                    {
                        // Als we de leveringsstatussen konden begrijpen, voegen we ze toe aan onze lijst
                        allStates.AddRange(states);
                    }
                }
                catch (JsonException ex)
                {
                    // Als er een fout is bij het begrijpen van de leveringsstatussen, loggen we de fout
                    Debug.WriteLine($"Error deserializing delivery states for order {i}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Voor andere fouten loggen we ook de fout
                    Debug.WriteLine($"Error getting delivery states for order {i}: {ex.Message}");
                }
            }
            // We geven alle leveringsstatussen terug die we hebben gevonden voor het opgegeven bereik van bestellingen
            return allStates;
        }

    }
}
