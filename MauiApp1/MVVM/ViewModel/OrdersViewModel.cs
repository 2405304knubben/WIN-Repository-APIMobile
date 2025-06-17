using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiApp1.ModelAPI;

namespace MauiApp1.ViewModel
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        private readonly ApiService.ApiService _apiService;
        private ObservableCollection<Order> _orders;
        private bool _isLoading;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        public OrdersViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _orders = new ObservableCollection<Order>();
            LoadOrdersCommand = new Command(async () => await LoadOrdersAsync());
            RefreshCommand = new Command(async () => await RefreshOrdersAsync());
            PropertyChanged = delegate { };
        }

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLoadOrders));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }

        public bool CanLoadOrders => !IsLoading;

        public ICommand LoadOrdersCommand { get; }
        public ICommand RefreshCommand { get; }        private async Task LoadOrdersAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var orders = await _apiService.GetOrdersAsync();
                Orders.Clear();
                foreach (var order in orders.OrderByDescending(o => o.OrderDate))
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading orders: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshOrdersAsync()
        {
            await LoadOrdersAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}