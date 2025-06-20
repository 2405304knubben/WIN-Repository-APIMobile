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
    }
}
