using Microsoft.Extensions.Logging;
using MauiApp1.ApiService;
using MauiApp1.Services;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MauiApp1.MVVM.Views;
using MauiApp1.MVVM.ViewModel;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Load configuration from embedded resource
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("MauiApp1.appsettings.json");

            if (stream == null)
            {
                throw new Exception("Failed to load appsettings.json. Ensure it is marked as an EmbeddedResource.");
            }

            builder.Configuration.AddJsonStream(stream);

            var apiSettings = builder.Configuration.GetSection("ApiSettings");
            var baseUrl = apiSettings["BaseUrl"];
            var apiKey = apiSettings["ApiKey"];

            var mapSettings = builder.Configuration.GetSection("MapSettings");
            var mapboxApiKey = mapSettings["MapboxApiKey"];

            if (string.IsNullOrWhiteSpace(mapboxApiKey))
            {
                throw new Exception("Mapbox API key is not configured in appsettings.json");
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new Exception("API BaseUrl is not configured in appsettings.json");
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("API Key is not configured in appsettings.json");
            }

#if DEBUG
            Console.WriteLine($"Loaded configuration - BaseUrl: {baseUrl}");
#endif

            // Register services in correct order
            builder.Services.AddSingleton(new ApiService.ApiService(apiKey, baseUrl));
            
            // Register MapboxService first, then MapService that depends on it
            var mapboxService = new Services.MapboxService(mapboxApiKey);
            builder.Services.AddSingleton(mapboxService);
            builder.Services.AddSingleton(new MapService(mapboxService));

            // Register views and viewmodels
            builder.Services.AddTransient<MVVM.Views.OrdersPage>();
            builder.Services.AddTransient<MVVM.ViewModel.OrdersPageViewModel>();
            builder.Services.AddTransient<MVVM.Views.DeliveryTrackingPage>();
            builder.Services.AddTransient(provider => new DeliveryTrackingPageViewModel(
                provider.GetRequiredService<ApiService.ApiService>(),
                provider.GetRequiredService<Services.MapboxService>(),
                mapboxApiKey,
                provider.GetRequiredService<MapService>()
            ));

            builder.Services.AddSingleton<AddOrderViewModel>();
            builder.Services.AddSingleton<AddOrderPage>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            return builder.Build();
        }
    }
}