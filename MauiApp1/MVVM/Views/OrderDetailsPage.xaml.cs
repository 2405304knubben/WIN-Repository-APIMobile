using System;
using Microsoft.Maui.Controls;
using MauiApp1.ModelAPI;
using MauiApp1.ApiService;
using System.Threading.Tasks;
using MauiApp1.MVVM.ViewModel;

namespace MauiApp1.MVVM.Views
{
    // Deze pagina laat alle details van één bestelling zien, zoals wat je hebt gekocht
    [QueryProperty(nameof(OrderId), "OrderId")]
    public partial class OrderDetailsPage : ContentPage
    {
        // Dit helpt ons om informatie van de computer te halen
        private readonly ApiService.ApiService? _apiService;
        private int orderId;
        
        // Het nummer van de bestelling die we willen bekijken
        public int OrderId
        {
            get => orderId;
            set
            {
                orderId = value;
                // Als we een nieuw nummer krijgen, halen we de informatie op
                LoadOrderDetails();
            }
        }
        
        // Hier maken we de pagina klaar om te gebruiken
        public OrderDetailsPage()
        {
            // We bouwen de pagina op met alle knoppen en teksten
            InitializeComponent();
            // We zorgen dat we informatie kunnen ophalen van de computer
            _apiService = Application.Current?.Handler?.MauiContext?.Services?.GetService<ApiService.ApiService>();
            // We vertellen de pagina waar hij zijn informatie vandaan moet halen
            BindingContext = new OrderDetailsPageViewModel();
        }
        
        // Hier halen we alle informatie over de bestelling op
        private async void LoadOrderDetails()
        {
            // Als we geen hulp hebben om informatie op te halen, stoppen we
            if (_apiService == null)
            {
                return;
            }
            // Als we geen geldig nummer hebben, stoppen we
            if (OrderId <= 0)
            {
                return;
            }
            // We halen alle bestellingen op van de computer
            var orders = await _apiService.GetOrdersAsync();
            // We zoeken naar de bestelling met het juiste nummer
            var found = orders?.Find(o => o.Id == OrderId);
            if (found != null)
            {
                // We geven de gevonden bestelling door aan onze pagina
                if (BindingContext is OrderDetailsPageViewModel vm)
                {
                    vm.Order = found;
                }
            }
        }
    }
}
