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
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private const int DefaultTimeoutSeconds = 30;

        public ApiService(string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be empty", nameof(apiKey));
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => 
                    {
                        #if DEBUG
                        return true; // In debug mode, accept all certificates
                        #else
                        return errors == System.Net.Security.SslPolicyErrors.None;
                        #endif
                    }
                };

                _client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(baseUrl.TrimEnd('/')),
                    Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds)
                };
                
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                _client.DefaultRequestHeaders.Add("ApiKey", apiKey);

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

        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = new List<Order>();
            // Define your range here
            int startId = 1200;
            int endId = 1300;

            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    var requestUrl = $"api/Order/{i}";
                    var response = await _client.GetAsync(requestUrl);
                    var content = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        // Optionally skip not found or log
                        Debug.WriteLine($"Order {i} not found or error: {response.StatusCode}");
                        continue;
                    }

                    var order = JsonSerializer.Deserialize<Order>(content, _jsonOptions);
                    if (order != null)
                    {
                        orders.Add(order);
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error deserializing order {i}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting order {i}: {ex.Message}");
                }
            }

            return orders;
        }

        public async Task<List<DeliveryState>> GetAllDeliveryStatesAsync()
        {
            try
            {
                var requestUrl = "api/DeliveryStates/GetAllDeliveryStates";
                var response = await _client.GetAsync(requestUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                response.EnsureSuccessStatusCode();

                var states = JsonSerializer.Deserialize<List<DeliveryState>>(content, _jsonOptions);
                return states ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching delivery states: {ex.Message}");
                throw;
            }
        }

        public async Task StartDeliveryAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/DeliveryStates/StartDelivery?OrderId={orderId}";
                var response = await _client.PostAsync(requestUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"StartDelivery Error Response: {content}");
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in StartDeliveryAsync: {ex.Message}");
                throw;
            }
        }

        public async Task CompleteDeliveryAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/DeliveryStates/CompleteDelivery?OrderId={orderId}";
                var response = await _client.PostAsync(requestUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"CompleteDelivery Error Response: {content}");
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CompleteDeliveryAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> PostOrderAsync(Order order)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(order, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("api/Order", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/Order/{orderId}";
                var response = await _client.GetAsync(requestUrl);
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
                var result = JsonSerializer.Deserialize<Order>(content, options);
                if (result == null)
                {
                    throw new JsonException("Failed to deserialize order from API response");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting order by id: {ex.Message}", ex);
            }
        }

        public async Task<List<DeliveryState>> FinishDeliveryAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/Order/{orderId}/FinishDelivery";
                var response = await _client.PostAsync(requestUrl, null);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode} - {content}");
                }

                var result = JsonSerializer.Deserialize<List<DeliveryState>>(content, _jsonOptions);
                return result ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in FinishDeliveryAsync: {ex}");
                throw;
            }
        }
        public async Task<List<DeliveryState>> GetDeliveryStatesByOrderRangeAsync(int startId, int endId)
        {
            var allStates = new List<DeliveryState>();
            for (int i = startId; i <= endId; i++)
            {
                try
                {
                    var requestUrl = $"api/DeliveryStates/GetByOrderId/{i}";
                    var response = await _client.GetAsync(requestUrl);
                    var content = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Delivery states for order {i} not found or error: {response.StatusCode}");
                        continue;
                    }

                    var states = JsonSerializer.Deserialize<List<DeliveryState>>(content, _jsonOptions);
                    if (states != null)
                    {
                        allStates.AddRange(states);
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error deserializing delivery states for order {i}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting delivery states for order {i}: {ex.Message}");
                }
            }
            return allStates;
        }

    }
}
