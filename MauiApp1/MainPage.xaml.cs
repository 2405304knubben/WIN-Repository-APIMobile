namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Login(object sender, EventArgs e)
        {
            LoginButton.Text = "Logging in...";
            SemanticScreenReader.Announce(LoginButton.Text);

            // Navigate to HomePage after login
            await Navigation.PushAsync(new HomePage());
        }
    }
}
