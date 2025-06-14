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
            if(string.IsNullOrEmpty(UsernameInput.Text) || string.IsNullOrEmpty(PasswordInput.Text))
            {
                DisplayAlert("Fout", "Vul alle velden in.", "OK");
                return;
            }

            NewPage1 newPage = new NewPage1();
            Navigation.PushAsync(newPage);
        }
    }
}
