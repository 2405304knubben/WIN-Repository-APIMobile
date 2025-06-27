using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.ModelAPI;
using System.Linq;

namespace MauiApp1.MVVM.ViewModel
{
    [QueryProperty(nameof(Order), "Order")]
    public partial class OrderDetailsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private Order? order;

        public decimal? TotaalPrijs => Order?.Products?.Sum(p => p.Price);
        public int? AantalProducten => Order?.Products?.Count;
        public int? AantalLeveringen => Order?.DeliveryStates?.Count;
    }
}
