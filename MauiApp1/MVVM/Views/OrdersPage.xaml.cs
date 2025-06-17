using MauiApp1.MVVM.ViewModel;
using MauiApp1.ApiService;

namespace MauiApp1.MVVM.Views
{
    public partial class OrdersPage : ContentPage
    {
        public OrdersPage()
        {
            InitializeComponent();
            BindingContext = new OrdersPageViewModel(
                Application.Current.Handler.MauiContext.Services.GetService<MauiApp1.ApiService.ApiService>());

            var viewModel = BindingContext as OrdersPageViewModel;
            if (viewModel != null)
            {
                viewModel.LoadOrdersCommand.Execute(null);
            }
        }
    }
}
