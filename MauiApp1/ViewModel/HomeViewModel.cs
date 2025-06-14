using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel; // Using CommunityToolkit for MVVM support (databinding en viewmodel ondersteuning)
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MauiApp1.ApiService; // Pas aan naar jouw namespace
using MauiApp1.ModelAPI;   // Pas aan naar jouw namespace

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    string welcomeMessage = "Welkom op de HomePage!";

    public ObservableCollection<DeliveryState> DeliveryStates { get; } = new();

    private readonly ApiService _apiService;

    public HomeViewModel(ApiService apiService)
    {
        _apiService = apiService;
        LoadDeliveryStates();
    }

    private async void LoadDeliveryStates()
    {
        var states = await _apiService.GetAllDeliveryStatesAsync();
        DeliveryStates.Clear();
        foreach (var state in states)
            DeliveryStates.Add(state);
    }
}


