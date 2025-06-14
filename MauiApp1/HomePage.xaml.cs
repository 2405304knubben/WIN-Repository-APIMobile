using MauiApp1.ViewModel;
using MauiApp1.ApiService;
using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        var apiService = Application.Current.Handler.MauiContext.Services.GetService<MauiApp1.ApiService.ApiService>();
        BindingContext = new HomeViewModel(apiService);
    }
}
