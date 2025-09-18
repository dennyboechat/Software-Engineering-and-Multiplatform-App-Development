using Microsoft.Extensions.Logging;
using LocationTracker.Services;

namespace LocationTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register database service
        builder.Services.AddSingleton<LocationDatabaseService>();

#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
        });
#endif

        return builder.Build();
    }
}