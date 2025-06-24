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

            // Load appsettings.json  
            // Add configuration from appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var apiSettings = builder.Configuration.GetSection("ApiSettings");
            var baseUrl = apiSettings["BaseUrl"];
            var apiKey = apiSettings["ApiKey"];

            var mapSettings = builder.Configuration.GetSection("MapSettings");
            var mapboxApiKey = mapSettings["MapboxApiKey"];

            if (string.IsNullOrWhiteSpace(mapboxApiKey))
            {
                throw new System.Exception("Mapbox API key is not configured in appsettings.json");
            }

            // Register services
            builder.Services.AddSingleton(new MauiApp1.ApiService.ApiService(apiKey ?? string.Empty, baseUrl ?? string.Empty));
            builder.Services.AddSingleton(new Services.MapboxService(mapboxApiKey));

            // Register views and viewmodels
            builder.Services.AddTransient<MVVM.Views.OrdersPage>();
            builder.Services.AddTransient<MVVM.ViewModel.OrdersPageViewModel>();
            builder.Services.AddTransient<MVVM.Views.DeliveryTrackingPage>();
            builder.Services.AddTransient(provider => new DeliveryTrackingPageViewModel(
                provider.GetRequiredService<ApiService.ApiService>(),
                provider.GetRequiredService<Services.MapboxService>(),
                mapboxApiKey
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