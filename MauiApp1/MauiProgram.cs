using Microsoft.Extensions.Logging;
using MauiApp1.ApiService;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Services.AddSingleton<MauiApp1.ApiService.ApiService>(sp =>
                new MauiApp1.ApiService.ApiService("e140c897-b374-4a2d-9b51-9516d92590f8", "http://51.137.100.120:5000/"));

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
