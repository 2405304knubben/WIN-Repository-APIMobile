using MauiApp1.MVVM.ViewModel;

namespace MauiApp1.MVVM.Views
{
    // Dit is een pagina waar je kunt zien waar je pakketje is, zoals wanneer je pizza wordt bezorgd
    public partial class DeliveryTrackingPage : ContentPage
    {
        // Hier maken we de pagina klaar om te gebruiken
        public DeliveryTrackingPage(DeliveryTrackingPageViewModel viewModel)
        {
            // We bouwen de pagina op met alle knoppen en teksten
            InitializeComponent();
            // We vertellen de pagina waar hij zijn informatie vandaan moet halen
            BindingContext = viewModel;
        }
    }
}
