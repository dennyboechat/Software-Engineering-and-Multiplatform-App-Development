using Microsoft.Extensions.Logging;

namespace LocationTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}