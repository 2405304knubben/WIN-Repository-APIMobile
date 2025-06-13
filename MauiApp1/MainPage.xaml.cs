namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void Login(object sender, EventArgs e)
        {
            LoginButton.Text = "Logging in...";
            // code voor in te loggen komt hier
            SemanticScreenReader.Announce(LoginButton.Text);
        }
    }
}
