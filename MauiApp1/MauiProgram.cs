using Microsoft.Extensions.Logging;
using MauiApp1.ApiService;
using MauiApp1.Services;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MauiApp1.MVVM.Views;
using MauiApp1.MVVM.ViewModel;

namespace MauiApp1
{
    // Dit is het startpunt van onze app, zoals het recept om een taart te maken
    public static class MauiProgram
    {
        // Deze functie maakt onze hele app klaar om te gebruiken
        public static MauiApp CreateMauiApp()
        {
            // We maken een bouwer die onze app gaat bouwen
            var builder = MauiApp.CreateBuilder();

            // We halen de instellingen op uit een bestand in onze app
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("MauiApp1.appsettings.json");

            // Als we het bestand niet kunnen vinden, stoppen we
            if (stream == null)
            {
                throw new Exception("Failed to load appsettings.json. Ensure it is marked as an EmbeddedResource.");
            }

            // We lezen alle instellingen uit het bestand
            builder.Configuration.AddJsonStream(stream);

            // We halen de instellingen voor de API op (waar we informatie vandaan halen)
            var apiSettings = builder.Configuration.GetSection("ApiSettings");
            var baseUrl = apiSettings["BaseUrl"];
            var apiKey = apiSettings["ApiKey"];

            // We halen de instellingen voor de kaarten op
            var mapSettings = builder.Configuration.GetSection("MapSettings");
            var mapboxApiKey = mapSettings["MapboxApiKey"];

            // We controleren of alle belangrijke instellingen er zijn
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
            // Als we de app aan het testen zijn, laten we zien welke instellingen we gebruiken
            Console.WriteLine($"Loaded configuration - BaseUrl: {baseUrl}");
#endif

            // We maken alle helpers klaar die onze app nodig heeft
            // Dit is als het klaarzetten van alle ingrediënten voor het koken
            
            // Helper om informatie van de server op te halen
            builder.Services.AddSingleton(new ApiService.ApiService(apiKey, baseUrl));
            
            // Helper om kaarten te laten zien (we maken deze eerst)
            var mapboxService = new Services.MapboxService(mapboxApiKey);
            builder.Services.AddSingleton(mapboxService);
            // Helper die afhankelijk is van de vorige helper
            builder.Services.AddSingleton(new MapService(mapboxService));

            // We maken alle pagina's en hun helpers klaar
            builder.Services.AddTransient<MVVM.Views.OrdersPage>();
            builder.Services.AddTransient<MVVM.ViewModel.OrdersPageViewModel>();
            builder.Services.AddTransient<MVVM.Views.DeliveryTrackingPage>();
            // Deze helper heeft meerdere andere helpers nodig
            builder.Services.AddTransient(provider => new DeliveryTrackingPageViewModel(
                provider.GetRequiredService<ApiService.ApiService>(),
                provider.GetRequiredService<Services.MapboxService>(),
                mapboxApiKey,
                provider.GetRequiredService<MapService>()
            ));

            // We bouwen de app en geven aan welke lettertypes we willen gebruiken
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // We voegen onze eigen lettertypes toe
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            
            // Nu is onze app klaar en kunnen we hem teruggeven
            return builder.Build();
        }
    }
}