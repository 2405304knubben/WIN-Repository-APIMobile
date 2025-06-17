using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService; // <-- Deze toevoegen
using MauiApp1.ModelAPI;   // <-- Ook nodig voor Order, Product, DeliveryState
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class AddOrderViewModel : ObservableObject
{
    [ObservableProperty]
    int newOrderCustomerId;

    [ObservableProperty]
    string newOrderStatus;

    private readonly ApiService _apiService;

    public AddOrderViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task PostOrder()
    {
        var order = new Order
        {
            CustomerId = NewOrderCustomerId,
            OrderDate = DateTime.Now,
            Products = new List<Product>(),
            DeliveryStates = new List<DeliveryState>()
        };

        var result = await _apiService.PostOrderAsync(order);
        if (result)
        {
            NewOrderStatus = "Order succesvol aangemaakt!";
            NewOrderCustomerId = 0;
        }
        else
        {
            NewOrderStatus = "Fout bij aanmaken order.";
        }
    }
}
