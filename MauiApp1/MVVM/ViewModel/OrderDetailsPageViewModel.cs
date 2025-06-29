using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.ModelAPI;
using System.Linq;

namespace MauiApp1.MVVM.ViewModel
{
    // Dit helpt de pagina om alle informatie over één bestelling te laten zien
    [QueryProperty(nameof(Order), "Order")]
    public partial class OrderDetailsPageViewModel : ObservableObject
    {
        // De bestelling waar we naar kijken
        [ObservableProperty]
        private Order? order;

        // Dit telt bij elkaar op hoeveel alle spullen in de bestelling kosten
        public decimal? TotaalPrijs => Order?.Products?.Sum(p => p.Price);
        // Dit telt hoeveel verschillende spullen er in de bestelling zitten
        public int? AantalProducten => Order?.Products?.Count;
        // Dit telt hoe vaak er iets is gebeurd met de bezorging
        public int? AantalLeveringen => Order?.DeliveryStates?.Count;
    }
}
