using MauiApp1.MVVM.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1.MVVM.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            var apiService = Application.Current.Handler.MauiContext.Services.GetService<MauiApp1.ApiService.ApiService>();
            BindingContext = new HomeViewModel(apiService);
        }
    }
}
