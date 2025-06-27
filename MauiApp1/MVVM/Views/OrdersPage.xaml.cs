using MauiApp1.MVVM.ViewModel;
using MauiApp1.ApiService;

namespace MauiApp1.MVVM.Views
{
    [QueryProperty(nameof(RefreshRequired), "RefreshRequired")]
    public partial class OrdersPage : ContentPage
    {
        private readonly OrdersPageViewModel _viewModel;
        private int _animatedCount = 0;
        private bool _refreshRequired;

        public bool RefreshRequired
        {
            get => _refreshRequired;
            set
            {
                _refreshRequired = value;
                if (value)
                {
                    ResetAndRefresh();
                }
            }
        }

        public OrdersPage(OrdersPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
        }

        private async void ResetAndRefresh()
        {
            _animatedCount = 0; // Reset animation counter
            await _viewModel.RefreshOrdersAsync();
            _refreshRequired = false;
        }

        // Tap animatie (schalen)
        private async void OnTapped(object sender, EventArgs e)
        {
            if (sender is View view)
            {
                await view.ScaleTo(0.95, 100);
                await view.ScaleTo(1, 100);
            }
        }

        // Fade-in + slide-in animatie voor de eerste 10 items
        private async void OrderBorder_Loaded(object sender, EventArgs e)
        {
            if (_animatedCount >= 10)
                return;

            if (sender is Border border)
            {
                border.TranslationX = -50;
                border.Opacity = 0;

                int index = _animatedCount;
                _animatedCount++;

                await Task.Delay(index * 100);

                var fadeTask = border.FadeTo(1, 350, Easing.CubicInOut);
                var moveTask = border.TranslateTo(0, 0, 350, Easing.CubicOut);
                await Task.WhenAll(fadeTask, moveTask);
            }
        }
    }
}
