using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiApp1.NewFolder;

namespace MauiApp1.ApiService
{
    // Services/ApiService.cs
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService(string apiKey, string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var response = await _client.GetAsync("orders");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Order>>(json);
        }

        // Voeg meer methodes toe voor andere API-calls
    }

}
