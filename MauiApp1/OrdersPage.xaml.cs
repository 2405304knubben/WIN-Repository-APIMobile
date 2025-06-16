using MauiApp1.ViewModel;
using MauiApp1.ApiService;

namespace MauiApp1;

public partial class OrdersPage : ContentPage
{
    public OrdersPage()
    {
        InitializeComponent();
        BindingContext = new OrdersPageViewModel(
            Application.Current.Handler.MauiContext.Services.GetService<ApiService.ApiService>());
    }
}
