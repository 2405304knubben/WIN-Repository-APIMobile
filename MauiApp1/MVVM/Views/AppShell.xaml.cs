using Microsoft.Maui.Controls;
using MauiApp1.MVVM.Views;

namespace MauiApp1
{
    // Dit is het hoofdscherm van onze app, zoals de voordeur van een huis
    public partial class AppShell : Shell
    {
        // Hier maken we het hoofdscherm klaar
        public AppShell()
        {
            // We bouwen het scherm op met alle knoppen en menu's
            InitializeComponent();

            // We vertellen de app hoe hij naar verschillende pagina's moet gaan
            // Zoals wegwijzers in een winkel die zeggen waar speelgoed en snoep staan
            Routing.RegisterRoute(nameof(OrderDetailsPage), typeof(OrderDetailsPage));
            Routing.RegisterRoute(nameof(DeliveryTrackingPage), typeof(DeliveryTrackingPage));

        
        }
    }
}
