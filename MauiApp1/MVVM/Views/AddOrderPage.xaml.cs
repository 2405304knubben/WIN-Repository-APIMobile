using MauiApp1.MVVM.ViewModel;
using MauiApp1.ApiService;

namespace MauiApp1.MVVM.Views;

public partial class AddOrderPage : ContentPage
{
    public AddOrderPage()
    {
        InitializeComponent();
        var apiService = Application.Current.Handler.MauiContext.Services.GetService<MauiApp1.ApiService.ApiService>();
        var vm = new AddOrderViewModel();
        vm.SetApiService(apiService);
        BindingContext = vm;
    }
}
