using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace LocationTracker;

public partial class MainPage : ContentPage
{
    private bool _isTracking = false;
    private System.Timers.Timer _trackingTimer;
    public ObservableCollection<LocationItem> LocationHistory { get; set; }

    public MainPage()
    {
        InitializeComponent();
        LocationHistory = new ObservableCollection<LocationItem>();
        LocationHistoryView.ItemsSource = LocationHistory;
        
        // Initialize timer for periodic tracking
        _trackingTimer = new System.Timers.Timer(10000); // 10 seconds
        _trackingTimer.Elapsed += async (sender, e) => await GetLocationAsync();
        
        // Check if location is available on startup
        CheckLocationAvailability();
    }

    private async void CheckLocationAvailability()
    {
        try
        {
            var isSupported = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(1)
            });
            
            if (isSupported != null)
            {
                UpdateStatus("Location services available - tap 'Get Current Location'");
            }
        }
        catch (FeatureNotSupportedException)
        {
            UpdateStatus("Location not supported on this device");
        }
        catch (FeatureNotEnabledException)
        {
            UpdateStatus("Location services are disabled - please enable in System Preferences");
        }
        catch (PermissionException)
        {
            UpdateStatus("Location permission required - tap 'Get Current Location' to grant");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Location check failed: {ex.Message}");
        }
    }

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        await GetLocationAsync();
    }

    private async void OnToggleTrackingClicked(object sender, EventArgs e)
    {
        if (!_isTracking)
        {
            await StartTrackingAsync();
        }
        else
        {
            StopTracking();
        }
    }

    private void OnClearHistoryClicked(object sender, EventArgs e)
    {
        LocationHistory.Clear();
        LocationMap.Pins.Clear();
        UpdateStatus("History cleared");
    }

    private async Task StartTrackingAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                _isTracking = true;
                ToggleTrackingBtn.Text = "Stop Tracking";
                ToggleTrackingBtn.BackgroundColor = Colors.Red;
                _trackingTimer.Start();
                UpdateStatus("Tracking started - updating every 10 seconds");
                
                // Get initial location
                await GetLocationAsync();
            }
            else
            {
                await DisplayAlert("Permission Denied", "Location permission is required for tracking.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to start tracking: {ex.Message}", "OK");
        }
    }

    private void StopTracking()
    {
        _isTracking = false;
        _trackingTimer.Stop();
        ToggleTrackingBtn.Text = "Start Tracking";
        ToggleTrackingBtn.BackgroundColor = Color.FromArgb("#2B0B98"); // Tertiary color
        UpdateStatus("Tracking stopped");
    }

    private async Task GetLocationAsync()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                UpdateStatus("Getting location...");
            });

            var request = new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(30)
            };

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var location = await Geolocation.GetLocationAsync(request, cancellationToken.Token);

            if (location != null)
            {
                var locationText = $"{location.Latitude:F6}, {location.Longitude:F6}";
                var timestamp = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                var accuracy = location.Accuracy?.ToString("F1") ?? "Unknown";

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LocationLabel.Text = locationText;
                    TimestampLabel.Text = $"Updated: {timestamp}";
                    
                    // Update map
                    UpdateMapLocation(location);
                    
                    // Add to history
                    var locationItem = new LocationItem
                    {
                        LocationText = locationText,
                        Timestamp = timestamp,
                        AccuracyText = $"±{accuracy}m",
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Accuracy = location.Accuracy ?? 0
                    };
                    
                    LocationHistory.Insert(0, locationItem); // Add to top of list
                    
                    // Add pin to map for history
                    AddLocationPin(location, timestamp);
                    
                    // Keep only last 50 locations
                    while (LocationHistory.Count > 50)
                    {
                        LocationHistory.RemoveAt(LocationHistory.Count - 1);
                    }
                    
                    UpdateStatus(_isTracking ? "Tracking active" : "Location updated");
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateStatus("Could not get location");
                });
            }
        }
        catch (FeatureNotSupportedException)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateStatus("Geolocation not supported on this device");
            });
        }
        catch (FeatureNotEnabledException)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateStatus("Geolocation is disabled");
            });
        }
        catch (PermissionException)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateStatus("Location permission not granted");
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateStatus($"Error: {ex.Message}");
            });
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            });
        }
    }

    private void UpdateStatus(string status)
    {
        StatusLabel.Text = status;
    }

    private void UpdateMapLocation(Location location)
    {
        var position = new Location(location.Latitude, location.Longitude);
        var mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1));
        LocationMap.MoveToRegion(mapSpan);
    }

    private void AddLocationPin(Location location, string timestamp)
    {
        var pin = new Pin
        {
            Label = "Location",
            Address = timestamp,
            Type = PinType.Place,
            Location = new Location(location.Latitude, location.Longitude)
        };
        
        LocationMap.Pins.Add(pin);
        
        // Keep only last 10 pins to avoid clutter
        while (LocationMap.Pins.Count > 10)
        {
            LocationMap.Pins.RemoveAt(0);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_isTracking)
        {
            StopTracking();
        }
    }
}

public class LocationItem
{
    public string LocationText { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string AccuracyText { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Accuracy { get; set; }
}