namespace MauiApp1;

public partial class AddOrderPage : ContentPage
{
    public AddOrderPage()
    {
        InitializeComponent();
        BindingContext = new AddOrderViewModel(
            Application.Current.Handler.MauiContext.Services.GetService<ApiService.ApiService>());
    }
}