namespace LocationTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register route for dependency injection
        Routing.RegisterRoute("MainPage", typeof(MainPage));
    }
}