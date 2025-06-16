using MauiApp1.ViewModel;
namespace MauiApp1;

public partial class OrdersPage : ContentPage
{
	public OrdersPage()
	{
		InitializeComponent();
		BindingContext = new ViewModel.OrdersViewModel(
            Application.Current.Handler.MauiContext.Services.GetService<ApiService.ApiService>());

        OrdersViewModel viewModel = BindingContext as ViewModel.OrdersViewModel;
        if (viewModel != null)
        {
           viewModel.LoadOrdersCommand.Execute(null);
        }
    }

}