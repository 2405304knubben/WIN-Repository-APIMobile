using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ModelAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp1.MVVM.ViewModel
{
    public partial class AddOrderViewModel : ObservableObject
    {
        [ObservableProperty]
        string newOrderStatus = string.Empty;

        [ObservableProperty]
        Customer newOrderCustomer = new Customer();

        [ObservableProperty]
        ObservableCollection<Product> newOrderProducts = new();

        [ObservableProperty]
        ObservableCollection<DeliveryState> newOrderDeliveryStates = new();

        // Temp properties for input
        [ObservableProperty]
        string tempProductName;
        [ObservableProperty]
        string tempProductDescription;
        [ObservableProperty]
        decimal tempProductPrice;

        [ObservableProperty]
        DeliveryStateEnum tempDeliveryStateEnum;
        [ObservableProperty]
        int tempDeliveryServiceId;

        public IEnumerable<DeliveryStateEnum> DeliveryStateEnumValues =>
            Enum.GetValues(typeof(DeliveryStateEnum)).Cast<DeliveryStateEnum>();

        private MauiApp1.ApiService.ApiService _apiService;

        // Parameterless constructor for XAML
        public AddOrderViewModel() { }

        // Optional: constructor for DI or testing
        public AddOrderViewModel(MauiApp1.ApiService.ApiService apiService)
        {
            _apiService = apiService;
        }

        // Setter for ApiService (call this in code-behind)
        public void SetApiService(MauiApp1.ApiService.ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        public void AddProduct()
        {
            if (!string.IsNullOrWhiteSpace(TempProductName))
            {
                NewOrderProducts.Add(new Product
                {
                    Name = TempProductName,
                    Description = TempProductDescription,
                    Price = TempProductPrice
                });
                TempProductName = string.Empty;
                TempProductDescription = string.Empty;
                TempProductPrice = 0;
            }
        }

        [RelayCommand]
        public void RemoveProduct(Product product)
        {
            if (NewOrderProducts.Contains(product))
                NewOrderProducts.Remove(product);
        }

        [RelayCommand]
        public void AddDeliveryState()
        {
            NewOrderDeliveryStates.Add(new DeliveryState
            {
                State = TempDeliveryStateEnum,
                DateTime = DateTime.Now,
                DeliveryServiceId = TempDeliveryServiceId
            });
            TempDeliveryStateEnum = default;
            TempDeliveryServiceId = 0;
        }

        [RelayCommand]
        public void RemoveDeliveryState(DeliveryState state)
        {
            if (NewOrderDeliveryStates.Contains(state))
                NewOrderDeliveryStates.Remove(state);
        }

        [RelayCommand]
        public async Task PostOrder()
        {
            if (_apiService == null)
            {
                NewOrderStatus = "ApiService niet ingesteld!";
                return;
            }

            var order = new Order
            {
                CustomerId = NewOrderCustomer.Id,
                Customer = NewOrderCustomer,
                OrderDate = DateTime.Now,
                Products = NewOrderProducts.ToList(),
                DeliveryStates = NewOrderDeliveryStates.ToList()
            };

            var result = await _apiService.PostOrderAsync(order);
            if (result)
            {
                NewOrderStatus = "Order succesvol aangemaakt!";
                NewOrderCustomer = new Customer();
                NewOrderProducts.Clear();
                NewOrderDeliveryStates.Clear();
            }
            else
            {
                NewOrderStatus = "Fout bij aanmaken order.";
            }
        }
    }
}
