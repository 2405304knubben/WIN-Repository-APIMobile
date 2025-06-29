// Dit bestand zorgt ervoor dat we alle bestellingen kunnen zien en filteren
// Het is als een grote lijst met alle pakketjes die bezorgd moeten worden
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.ApiService;
using MauiApp1.ModelAPI;
using System.Collections.ObjectModel;

namespace MauiApp1.MVVM.ViewModel
{
    // Dit zijn de verschillende manieren waarop we bestellingen kunnen filteren
    public enum OrdersFilter
    {
        // Alle bestellingen
        All,
        // Alleen bestellingen die wachten
        Pending,
        // Alleen bestellingen die onderweg zijn
        InTransit,
        // Alleen bestellingen die bezorgd zijn
        Delivered
    }

    // Deze klasse zorgt voor alles wat met het tonen van bestellingen te maken heeft
    public partial class OrdersPageViewModel : ObservableObject
    {
        // Dit gebruiken we om bestellingen op te halen van de server
        private readonly ApiService.ApiService _apiService;
        // Dit is onze lijst met alle bestellingen
        private List<Order> _allOrders = new();

        // Dit is de lijst met bestellingen die we laten zien
        public ObservableCollection<Order> Orders { get; } = new();

        // Dit is een berichtje dat we kunnen laten zien (bijvoorbeeld "Laden...")
        private string? _statusMessage;
        public string? StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Dit is het filter dat nu actief is (Alle, Wachtend, Onderweg, etc.)
        private OrdersFilter _currentFilter = OrdersFilter.All;
        public OrdersFilter CurrentFilter
        {
            get => _currentFilter;
            set
            {
                if (SetProperty(ref _currentFilter, value))
                {
                    OnPropertyChanged(nameof(AllOrdersButtonOpacity));
                    OnPropertyChanged(nameof(PendingButtonOpacity));
                    OnPropertyChanged(nameof(InTransitButtonOpacity));
                    OnPropertyChanged(nameof(DeliveredButtonOpacity));
                    ApplyFilter();
                }
            }
        }

        // Dit is de tekst waarmee we bestellingen kunnen zoeken
        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        // Dit zegt of we alleen bestellingen van vandaag willen zien
        private bool _onlyToday;
        public bool OnlyToday
        {
            get => _onlyToday;
            set
            {
                if (SetProperty(ref _onlyToday, value))
                {
                    ApplyFilter();
                }
            }
        }

        // Dit zegt of we nog bezig zijn met laden
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Deze waardes bepalen hoe duidelijk de filterknoppen te zien zijn
        // De actieve knop is helemaal zichtbaar, de andere knoppen zijn een beetje doorzichtig
        public double AllOrdersButtonOpacity => CurrentFilter == OrdersFilter.All ? 1.0 : 0.5;
        public double PendingButtonOpacity => CurrentFilter == OrdersFilter.Pending ? 1.0 : 0.5;
        public double InTransitButtonOpacity => CurrentFilter == OrdersFilter.InTransit ? 1.0 : 0.5;
        public double DeliveredButtonOpacity => CurrentFilter == OrdersFilter.Delivered ? 1.0 : 0.5;

        // Dit wordt uitgevoerd als we de pagina maken
        public OrdersPageViewModel(ApiService.ApiService apiService)
        {
            _apiService = apiService;
            // We laden meteen alle bestellingen
            LoadOrdersCommand.Execute(null);
        }

        // Deze functie wordt gebruikt om de lijst met bestellingen opnieuw te laden
        public async Task RefreshOrdersAsync()
        {
            await LoadOrdersCommand.ExecuteAsync(null);
        }

        // Deze functie wordt uitgevoerd als we naar een andere pagina gaan
        private void Current_Navigating(object? sender, ShellNavigatingEventArgs e)
        {
            // Als we naar de bestellingen-pagina gaan, laden we alles opnieuw
            if (e.Current?.Location?.ToString().EndsWith("OrdersPage") == true)
            {
                LoadOrdersCommand.Execute(null);
            }
        }

        // Deze functie haalt alle bestellingen op en laat ze zien
        [RelayCommand]
        public async Task LoadOrders()
        {
            try
            {
                // Eerst maken we alle lijsten leeg
                Orders.Clear();
                _allOrders.Clear();
                // We laten zien dat we aan het laden zijn
                IsLoading = true;
                StatusMessage = "Ophalen van orders...";

                // We wachten heel even zodat mensen kunnen zien dat we laden
                await Task.Delay(300);

                // We halen alle bestellingen en hun statussen op
                var ordersTask = _apiService.GetOrdersAsync();
                var statesTask = _apiService.GetAllDeliveryStatesAsync();

                // We wachten tot we alles hebben
                await Task.WhenAll(ordersTask, statesTask);

                var orders = ordersTask.Result;
                var allStates = statesTask.Result;

                // We zorgen dat elke bestelling zijn eigen statussen krijgt
                var statesByOrderId = allStates.GroupBy(s => s.OrderId)
                                               .ToDictionary(g => g.Key, g => g.ToList());

                if (orders != null)
                {
                    // Voor elke bestelling...
                    foreach (var order in orders)
                    {
                        // Zoeken we de bijbehorende statussen
                        if (statesByOrderId.TryGetValue(order.Id, out var states))
                            order.DeliveryStates = states;
                        else
                            order.DeliveryStates = new List<DeliveryState>();
                        _allOrders.Add(order);
                    }
                    
                    // We wachten nog heel even voordat we alles laten zien
                    await Task.Delay(200);
                    ApplyFilter();
                }

                if (!Orders.Any())
                    StatusMessage = "Geen orders gevonden.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fout bij ophalen orders: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Deze knoppen zijn voor de verschillende filters
        // Elke knop past het filter aan zodat we alleen bepaalde bestellingen zien
        [RelayCommand]
        private void ShowAllOrders() { CurrentFilter = OrdersFilter.All; ApplyFilter(); }

        [RelayCommand]
        private void ShowPendingOrders() { CurrentFilter = OrdersFilter.Pending; ApplyFilter(); }

        [RelayCommand]
        private void ShowInTransitOrders() { CurrentFilter = OrdersFilter.InTransit; ApplyFilter(); }

        [RelayCommand]
        private void ShowDeliveredOrders() { CurrentFilter = OrdersFilter.Delivered; ApplyFilter(); }

        // Deze functie zorgt dat we alleen de bestellingen zien die we willen zien
        // Het is als een zeef die alleen bepaalde dingen doorlaat
        private void ApplyFilter()
        {
            // We maken de lijst leeg zodat we opnieuw kunnen beginnen
            Orders.Clear();
            // We beginnen met alle bestellingen
            IEnumerable<Order> filtered = _allOrders;

            // We kijken welk filter actief is en houden alleen die bestellingen over
            switch (CurrentFilter)
            {
                case OrdersFilter.Pending:
                    // Alleen bestellingen die nog niet gestart zijn
                    filtered = filtered.Where(o =>
                        o.DeliveryStates == null ||
                        !o.DeliveryStates.Any() ||
                        o.DeliveryStates.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State == DeliveryStateEnum.Pending
                    );
                    StatusMessage = $"Niet gestarte bestellingen ({filtered.Count()})";
                    break;
                case OrdersFilter.InTransit:
                    // Alleen bestellingen die onderweg zijn
                    filtered = filtered.Where(o =>
                        o.DeliveryStates != null &&
                        o.DeliveryStates.Any() &&
                        o.DeliveryStates.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State == DeliveryStateEnum.InTransit
                    );
                    StatusMessage = $"Onderweg ({filtered.Count()})";
                    break;
                case OrdersFilter.Delivered:
                    // Alleen bestellingen die al bezorgd zijn
                    filtered = filtered.Where(o =>
                        o.DeliveryStates != null &&
                        o.DeliveryStates.Any() &&
                        o.DeliveryStates.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State == DeliveryStateEnum.Delivered
                    );
                    StatusMessage = $"Bezorgd ({filtered.Count()})";
                    break;
                case OrdersFilter.All:
                default:
                    // Alle bestellingen
                    StatusMessage = $"Alle bestellingen ({filtered.Count()})";
                    break;
            }

            // Als we alleen vandaag willen zien, filteren we op datum
            if (OnlyToday)
            {
                var today = DateTime.Today;
                filtered = filtered.Where(o => o.OrderDate.Date == today);
                StatusMessage += " (vandaag)";
            }

            // Als we zoektekst hebben, zoeken we daarmee
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var lower = SearchText.ToLower();
                filtered = filtered.Where(o =>
                    // We zoeken op bestelling nummer of klant naam
                    o.Id.ToString() == lower ||
                    (o.Customer?.Name?.ToLower().Contains(lower) ?? false)
                );
            }

            // We sorteren de bestellingen: bezorgde bestellingen onderaan, nieuwste eerst
            foreach (var order in filtered
                .OrderBy(o => GetLastState(o) == DeliveryStateEnum.Delivered)
                .ThenByDescending(o => o.OrderDate))
            {
                Orders.Add(order);
            }
        }

        // Deze opmerking klopt: we hebben de partial methods niet meer nodig

        // Deze functie kijkt wat de laatste status van een bestelling is
        private DeliveryStateEnum? GetLastState(Order order)
        {
            // We zoeken de nieuwste status van de bestelling
            return order.DeliveryStates?.OrderByDescending(s => s.DateTime).FirstOrDefault()?.State;
        }

        // Deze functie brengt ons naar de pagina waar we de bezorging kunnen volgen
        [RelayCommand]
        private async Task GoToDeliveryTracking(Order order)
        {
            // Als er geen bestelling is, doen we niets
            if (order == null)
                return;

            // We maken een pakketje met informatie om mee te nemen naar de volgende pagina
            var parameters = new Dictionary<string, object>
            {
                { "Order", order }
            };

            // We gaan naar de bezorg-volg pagina
            await Shell.Current.GoToAsync($"DeliveryTrackingPage", parameters);
        }
    }
}
