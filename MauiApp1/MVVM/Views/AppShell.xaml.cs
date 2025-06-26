using Microsoft.Maui.Controls;
using MauiApp1.MVVM.Views;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(OrderDetailsPage), typeof(OrderDetailsPage));
            Routing.RegisterRoute(nameof(DeliveryTrackingPage), typeof(DeliveryTrackingPage));

        
        }
    }
}
