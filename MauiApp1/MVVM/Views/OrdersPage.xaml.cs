using MauiApp1.MVVM.ViewModel;
using MauiApp1.ApiService;

namespace MauiApp1.MVVM.Views
{
    // Deze pagina laat alle bestellingen zien, zoals een lijst van alle speelgoed dat je hebt besteld
    [QueryProperty(nameof(RefreshRequired), "RefreshRequired")]
    public partial class OrdersPage : ContentPage
    {
        // Dit helpt ons om de informatie op het scherm te zetten
        private readonly OrdersPageViewModel _viewModel;
        // Dit houdt bij hoeveel dingen we al hebben laten bewegen op het scherm
        private int _animatedCount = 0;
        private bool _refreshRequired;

        // Dit vertelt ons of we nieuwe informatie moeten ophalen
        public bool RefreshRequired
        {
            get => _refreshRequired;
            set
            {
                _refreshRequired = value;
                if (value)
                {
                    // Als we nieuwe informatie nodig hebben, halen we die op
                    ResetAndRefresh();
                }
            }
        }

        // Hier maken we de pagina klaar om te gebruiken
        public OrdersPage(OrdersPageViewModel viewModel)
        {
            // We bouwen de pagina op met alle knoppen en teksten
            InitializeComponent();
            // We bewaren onze helper om informatie te tonen
            _viewModel = viewModel;
            // We vertellen de pagina waar hij zijn informatie vandaan moet halen
            BindingContext = _viewModel;
        }

        // Dit gebeurt elke keer als de pagina op het scherm komt
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
        }

        // Hier maken we alles schoon en halen nieuwe informatie op
        private async void ResetAndRefresh()
        {
            // We beginnen weer opnieuw met tellen voor de bewegende dingen
            _animatedCount = 0;
            // We vragen om nieuwe informatie over alle bestellingen
            await _viewModel.RefreshOrdersAsync();
            // We zijn klaar met verversen
            _refreshRequired = false;
        }

        // Dit laat dingen bewegen als je erop drukt, zoals een knop die ingedrukt wordt
        private async void OnTapped(object sender, EventArgs e)
        {
            if (sender is View view)
            {
                // Eerst wordt het iets kleiner
                await view.ScaleTo(0.95, 100);
                // Dan wordt het weer normaal groot
                await view.ScaleTo(1, 100);
            }
        }

        // Dit laat dingen mooi verschijnen op het scherm, zoals toveren
        private async void OrderBorder_Loaded(object sender, EventArgs e)
        {
            // We laten alleen de eerste 10 dingen bewegen, anders wordt het te druk
            if (_animatedCount >= 10)
                return;

            if (sender is Border border)
            {
                // We zetten het ding eerst onzichtbaar en naar links
                border.TranslationX = -50;
                border.Opacity = 0;

                // We onthouden welk nummer dit ding heeft
                int index = _animatedCount;
                _animatedCount++;

                // We wachten een beetje zodat niet alles tegelijk beweegt
                await Task.Delay(index * 100);

                // Nu laten we het zichtbaar worden en naar de goede plek schuiven
                var fadeTask = border.FadeTo(1, 350, Easing.CubicInOut);
                var moveTask = border.TranslateTo(0, 0, 350, Easing.CubicOut);
                await Task.WhenAll(fadeTask, moveTask);
            }
        }
    }
}
