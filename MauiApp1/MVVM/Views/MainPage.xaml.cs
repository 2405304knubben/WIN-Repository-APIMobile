using MauiApp1.MVVM.ViewModel;

namespace MauiApp1.MVVM.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
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

        private async void Login(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameInput.Text) || string.IsNullOrWhiteSpace(PasswordInput.Text))
            {
                await DisplayAlert("Error", "Vul aub je gebruikesnaam en wachtwoord in.", "OK");
                return;
            }
            LoginButton.Text = "Aan het inloggen...";
            SemanticScreenReader.Announce(LoginButton.Text);

            // Navigate to HomePage after login
            await Navigation.PushAsync(new HomePage());
        }
    }
}
