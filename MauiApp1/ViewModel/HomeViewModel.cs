using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;

namespace MauiApp1.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        string welcomeMessage = "Welkom op de HomePage!";

        [ObservableProperty]
        string statusMessage; // <-- Toegevoegd

        public ObservableCollection<DeliveryState> DeliveryStates { get; } = new();

        private readonly ApiService.ApiService _apiService;

        public HomeViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
            LoadDeliveryStates();
        }

        private async void LoadDeliveryStates()
        {
            try
            {
                StatusMessage = "Ophalen van DeliveryStates...";
                var states = await _apiService.GetAllDeliveryStatesAsync();
                StatusMessage = $"Aantal opgehaalde DeliveryStates: {states?.Count ?? 0}";

                DeliveryStates.Clear();
                foreach (var state in states)
                    DeliveryStates.Add(state);

                if (states == null || states.Count == 0)
                    StatusMessage = "Geen DeliveryStates ontvangen van de API.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij ophalen DeliveryStates: {ex.Message}";
            }
        }
    }
}
