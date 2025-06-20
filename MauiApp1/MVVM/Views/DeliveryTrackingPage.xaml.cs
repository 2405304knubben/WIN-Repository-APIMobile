using MauiApp1.MVVM.ViewModel;

namespace MauiApp1.MVVM.Views
{
    public partial class DeliveryTrackingPage : ContentPage
    {
        public DeliveryTrackingPage(DeliveryTrackingPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
