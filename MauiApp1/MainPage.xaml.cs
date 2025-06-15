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
            if(string.IsNullOrWhiteSpace(UsernameInput.Text) || string.IsNullOrWhiteSpace(PasswordInput.Text))
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
