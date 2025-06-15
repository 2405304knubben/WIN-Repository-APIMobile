using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;

namespace MauiApp1.ApiService
{
    public class ApiService
    {
        private readonly HttpClient _client;

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
                
                return JsonSerializer.Deserialize<List<Order>>(content) ?? new List<Order>();
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
                var requestUrl = "api/DeliveryStates/DeliveryStates";
                var response = await _client.GetAsync(requestUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
                
                return JsonSerializer.Deserialize<List<DeliveryState>>(content) ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting delivery states: {ex.Message}", ex);
            }
        }

        public async Task<List<DeliveryState>> StartDeliveryAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/DeliveryStates/StartDelivery?OrderId={orderId}";
                var response = await _client.PostAsync(requestUrl, null);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
                
                return JsonSerializer.Deserialize<List<DeliveryState>>(content) ?? new List<DeliveryState>();
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
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
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
        }

        public async Task<List<DeliveryState>> CompleteDeliveryAsync(int orderId)
        {
            try
            {
                var requestUrl = $"api/DeliveryStates/CompleteDelivery?OrderId={orderId}";
                var response = await _client.PostAsync(requestUrl, null);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error {(int)response.StatusCode}: {content}");
                }
                
                return JsonSerializer.Deserialize<List<DeliveryState>>(content) ?? new List<DeliveryState>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error completing delivery: {ex.Message}", ex);
            }
        }
    }
}
