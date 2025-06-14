using Microsoft.Extensions.Logging;
using MauiApp1.ApiService;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // In MauiProgram.cs
            builder.Services.AddSingleton<ApiService.ApiService>(sp =>
                    new ApiService.ApiService("e140c897-b374-4a2d-9b51-9516d92590f8", "http://51.137.100.120:5000/swagger/index.html"));

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
