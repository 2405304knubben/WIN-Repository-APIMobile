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

        public ApiService(string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be empty", nameof(apiKey));
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));

            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl.TrimEnd('/'))
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
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            try
            {
                var requestUrl = "api/Order";
                var response = await _client.GetAsync(requestUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }

                var result = JsonSerializer.Deserialize<List<Order>>(content, _jsonOptions);
                return result ?? new List<Order>();
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error deserializing orders response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting orders: {ex.Message}", ex);
            }
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

        public async Task<List<DeliveryState>> StartDeliveryAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/DeliveryStates/StartDelivery?OrderId={orderId}";
                var response = await _client.PostAsync(requestUrl, null);
                var content = await response.Content.ReadAsStringAsync();

                // Log the response for debugging
                System.Diagnostics.Debug.WriteLine($"StartDelivery Response: {content}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }

                // Deserialize als enkele DeliveryState
                var singleState = JsonSerializer.Deserialize<DeliveryState>(content, _jsonOptions);
                return singleState != null ? new List<DeliveryState> { singleState } : new List<DeliveryState>();
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error deserializing start delivery response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error starting delivery: {ex.Message}", ex);
            }
        }

        public async Task<List<DeliveryState>> UpdateDeliveryStateAsync(DeliveryState deliveryState)
        {
            try
            {
                var requestUrl = "api/DeliveryStates/UpdateDeliveryState";
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                var content = JsonSerializer.Serialize(deliveryState, options);
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(requestUrl, httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {responseContent}");
                }
                return JsonSerializer.Deserialize<List<DeliveryState>>(responseContent, options) ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating delivery state: {ex.Message}", ex);
            }
        }        public async Task<List<DeliveryState>> CompleteDeliveryAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/DeliveryStates/CompleteDelivery/{orderId}";
                var response = await _client.PostAsync(requestUrl, null);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Complete delivery response: {content}");
                
                var states = JsonSerializer.Deserialize<List<DeliveryState>>(content, _jsonOptions);
                return states ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error completing delivery: {ex}");
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

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            try
            {
                var requestUrl = $"api/Order/{id}";
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
                return JsonSerializer.Deserialize<Order>(content, options);
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

    }
}
