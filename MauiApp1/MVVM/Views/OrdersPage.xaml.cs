using MauiApp1.MVVM.ViewModel;
using MauiApp1.ApiService;

namespace MauiApp1.MVVM.Views
{
    public partial class OrdersPage : ContentPage
    {        private readonly OrdersPageViewModel _viewModel;
        
        public OrdersPage(OrdersPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private async void OnTapped(object sender, EventArgs e)
        {
            var view = sender as View;
            await view.ScaleTo(0.95, 100);
            await view.ScaleTo(1, 100);
        }

    }
}
