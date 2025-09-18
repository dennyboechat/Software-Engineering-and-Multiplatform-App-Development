using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using LocationTracker.Services;
using LocationTracker.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LocationTracker;

public partial class MainPage : ContentPage
{
    private bool _isTracking = false;
    private bool _showHeatMap = false;
    private bool _showPins = true;
    private bool _isSimulating = false;
    private System.Timers.Timer _trackingTimer;
    private System.Timers.Timer _simulationTimer;
    private HeatMapData _heatMapData = new();
    private Location? _currentSimulatedLocation = null;
    private int _simulationStep = 0;
    private const int _totalSimulationSteps = 20; // 1 minute / 3 seconds per step
    private readonly LocationDatabaseService _databaseService;
    private string _currentSessionId = string.Empty;
    public ObservableCollection<LocationItem> LocationHistory { get; set; }

    public MainPage()
    {
        InitializeComponent();
        
        // Get database service from service provider with error handling
        try
        {
            _databaseService = Application.Current?.Handler?.MauiContext?.Services?.GetService<LocationDatabaseService>();
            if (_databaseService == null)
            {
                // Fallback: create new instance if service locator fails
                _databaseService = new LocationDatabaseService();
            }
        }
        catch (Exception ex)
        {
            // Fallback: create new instance if any error occurs
            _databaseService = new LocationDatabaseService();
            System.Diagnostics.Debug.WriteLine($"Database service injection failed: {ex.Message}");
        }
        
        LocationHistory = new ObservableCollection<LocationItem>();
        LocationHistoryView.ItemsSource = LocationHistory;
        
        // Initialize timer for periodic tracking
        _trackingTimer = new System.Timers.Timer(10000); // 10 seconds
        _trackingTimer.Elapsed += async (sender, e) => await GetLocationAsync();
        
                // Initialize simulation timer
        _simulationTimer = new System.Timers.Timer(3000); // 3 second intervals
        _simulationTimer.AutoReset = true;
        _simulationTimer.Elapsed += async (sender, e) => await SimulateNextLocationStep();
        
        // Initialize database and load existing locations
        _ = Task.Run(async () => await InitializeDatabaseAsync());
        
        // Check if location is available on startup
        CheckLocationAvailability();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            if (_databaseService != null)
            {
                await _databaseService.InitializeAsync();
                
                // Generate new session ID for this app session
                _currentSessionId = Guid.NewGuid().ToString("N")[..8]; // Short 8-character ID
                
                // Load recent locations from database
                await LoadRecentLocationsAsync();
                
                System.Diagnostics.Debug.WriteLine($"Database initialized successfully. Session ID: {_currentSessionId}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Database service is null, skipping database initialization");
                _currentSessionId = Guid.NewGuid().ToString("N")[..8]; // Still need session ID
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            _currentSessionId = Guid.NewGuid().ToString("N")[..8]; // Still need session ID
            
            // Initialize empty history on UI thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LocationHistory.Clear();
                UpdateStatus("Database unavailable - running in memory mode");
            });
        }
    }

    private async Task LoadRecentLocationsAsync()
    {
        try
        {
            var recentLocations = await _databaseService.GetRecentLocationsAsync(20);
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LocationHistory.Clear();
                
                foreach (var record in recentLocations)
                {
                    var locationItem = new LocationItem
                    {
                        LocationText = record.LocationText,
                        Timestamp = $"{record.DisplayType} {record.TimestampText}",
                        AccuracyText = record.AccuracyText,
                        Latitude = record.Latitude,
                        Longitude = record.Longitude,
                        Accuracy = record.Accuracy ?? 0
                    };
                    
                    LocationHistory.Add(locationItem);
                    
                    // Add pins for stored locations
                    AddLocationPin(new Location(record.Latitude, record.Longitude), record.DisplayType);
                }
                
                if (recentLocations.Any())
                {
                    UpdateStatus($"Loaded {recentLocations.Count} locations from database");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load recent locations: {ex.Message}");
        }
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
        // _heatMapData.ClearPoints();
        // UpdateHeatMapDisplay();
        UpdateStatus("History cleared");
    }

    /* Heat map functionality temporarily disabled
    private void OnToggleHeatMapClicked(object sender, EventArgs e)
    {
        _showHeatMap = !_showHeatMap;
        HeatMapOverlay.IsVisible = _showHeatMap;
        ToggleHeatMapBtn.Text = _showHeatMap ? "Hide Heat Map" : "Show Heat Map";
        ToggleHeatMapBtn.BackgroundColor = _showHeatMap ? Colors.Orange : Color.FromArgb("#DFD8F7");
        
        if (_showHeatMap)
        {
            UpdateHeatMapDisplay();
            UpdateStatus("Heat map enabled");
        }
        else
        {
            UpdateStatus("Heat map disabled");
        }
    }

    private void OnToggleViewClicked(object sender, EventArgs e)
    {
        _showPins = !_showPins;
        ToggleViewBtn.Text = _showPins ? "Hide Pins" : "Show Pins";
        ToggleViewBtn.BackgroundColor = _showPins ? Colors.LightGreen : Color.FromArgb("#ACACAC");
        
        if (!_showPins)
        {
            LocationMap.Pins.Clear();
            UpdateStatus("Location pins hidden");
        }
        else
        {
            // Re-add pins for recent locations
            LocationMap.Pins.Clear();
            var recentLocations = LocationHistory.Take(10);
            foreach (var item in recentLocations)
            {
                AddLocationPin(new Location(item.Latitude, item.Longitude), item.Timestamp);
            }
            UpdateStatus("Location pins shown");
        }
    }
    */

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
                    
                    // Add to heat map data - Temporarily disabled
                    // _heatMapData.AddPoint(location);
                    // UpdateHeatMapDisplay();
                    
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
                
                // Save to database
                await SaveLocationToDatabase(location, false);
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

    private async Task SaveLocationToDatabase(Location location, bool isSimulated)
    {
        try
        {
            if (_databaseService == null)
            {
                System.Diagnostics.Debug.WriteLine("Database service not available, skipping location save");
                return;
            }

            var locationRecord = new LocationRecord
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Altitude = location.Altitude,
                Accuracy = location.Accuracy,
                Speed = location.Speed,
                Heading = location.Course,
                Timestamp = DateTime.Now,
                TrackingType = isSimulated ? "SIM" : "GPS",
                SessionId = _currentSessionId ?? "unknown",
                IsSimulated = isSimulated,
                Notes = isSimulated ? $"Simulation step {_simulationStep}" : ""
            };

            await _databaseService.SaveLocationAsync(locationRecord);
            System.Diagnostics.Debug.WriteLine($"Saved location to database: {locationRecord.LocationText} (Session: {_currentSessionId})");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save location to database: {ex.Message}. Continuing without database.");
        }
    }

    private void UpdateStatus(string status)
    {
        StatusLabel.Text = status;
    }

    private void UpdateMapLocation(Location location)
    {
        var position = new Location(location.Latitude, location.Longitude);
        // Use larger radius for simulation to show the extended walking path
        var radius = _isSimulating ? Distance.FromMeters(600) : Distance.FromMeters(200);
        var mapSpan = MapSpan.FromCenterAndRadius(position, radius);
        LocationMap.MoveToRegion(mapSpan);
        
        // Debug: Log map update
        System.Diagnostics.Debug.WriteLine($"Updated map center to: {location.Latitude:F6}, {location.Longitude:F6}");
    }

    private void AddLocationPin(Location location, string timestamp)
    {
        var pin = new Pin
        {
            Label = $"📍 {timestamp}",
            Address = $"{location.Latitude:F6}, {location.Longitude:F6}",
            Type = PinType.Place,
            Location = new Location(location.Latitude, location.Longitude)
        };
        
        LocationMap.Pins.Add(pin);
        
        // Debug: Log pin addition
        System.Diagnostics.Debug.WriteLine($"Added pin: {timestamp} at {location.Latitude:F6}, {location.Longitude:F6}. Total pins: {LocationMap.Pins.Count}");
        
        // Keep only last 20 pins to show more of the walking path
        while (LocationMap.Pins.Count > 20)
        {
            LocationMap.Pins.RemoveAt(0);
        }
    }

    /* Heat map update method temporarily disabled
    private void UpdateHeatMapDisplay()
    {
        if (_showHeatMap && _heatMapData.Points.Any())
        {
            HeatMapOverlay.HeatMapData = _heatMapData;
            if (LocationMap.VisibleRegion != null)
            {
                HeatMapOverlay.MapSpan = LocationMap.VisibleRegion;
            }
            HeatMapOverlay.InvalidateSurface();
        }
    }
    */

    private async void OnSimulateWalkingClicked(object sender, EventArgs e)
    {
        if (_isSimulating) return;

        try
        {
            // Get current location as starting point for simulation
            UpdateStatus("Getting starting location for simulation...");
            
            var request = new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            };

            var location = await Geolocation.GetLocationAsync(request);
            
            if (location != null)
            {
                _currentSimulatedLocation = location;
                StartWalkingSimulation();
            }
            else
            {
                // Use default location (San Francisco) if GPS not available
                _currentSimulatedLocation = new Location(37.7749, -122.4194);
                StartWalkingSimulation();
            }
        }
        catch (Exception ex)
        {
            // Use default location if any error occurs
            _currentSimulatedLocation = new Location(37.7749, -122.4194);
            StartWalkingSimulation();
        }
    }

    private void OnStopSimulationClicked(object sender, EventArgs e)
    {
        StopWalkingSimulation();
    }

    private void StartWalkingSimulation()
    {
        _isSimulating = true;
        _simulationStep = 0;
        
        // Clear existing pins to start fresh
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LocationMap.Pins.Clear();
            SimulateWalkingBtn.IsVisible = false;
            StopSimulationBtn.IsVisible = true;
            UpdateStatus("Starting walking simulation (1 minute)...");
        });

        // Start simulation timer
        _simulationTimer.Start();
        
        // Add initial location
        if (_currentSimulatedLocation != null)
        {
            System.Diagnostics.Debug.WriteLine($"Starting simulation at: {_currentSimulatedLocation.Latitude:F6}, {_currentSimulatedLocation.Longitude:F6}");
            ProcessSimulatedLocation(_currentSimulatedLocation);
        }
    }

    private void StopWalkingSimulation()
    {
        _isSimulating = false;
        _simulationTimer.Stop();
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            SimulateWalkingBtn.IsVisible = true;
            StopSimulationBtn.IsVisible = false;
            UpdateStatus("Walking simulation stopped");
        });
    }

    private async Task SimulateNextLocationStep()
    {
        if (!_isSimulating || _currentSimulatedLocation == null)
            return;

        _simulationStep++;
        
        // Generate next location point (simulating walking)
        var nextLocation = GenerateNextWalkingLocation(_currentSimulatedLocation, _simulationStep);
        _currentSimulatedLocation = nextLocation;
        
        System.Diagnostics.Debug.WriteLine($"Simulation Step {_simulationStep}: Moving to {nextLocation.Latitude:F6}, {nextLocation.Longitude:F6}");
        
        // Process the simulated location
        ProcessSimulatedLocation(nextLocation);
        
        // Check if simulation is complete (1 minute = 20 steps of 3 seconds each)
        if (_simulationStep >= _totalSimulationSteps)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StopWalkingSimulation();
                UpdateStatus($"Walking simulation completed! Tracked {_simulationStep} points over 1 minute.");
            });
        }
    }

    private Location GenerateNextWalkingLocation(Location currentLocation, int step)
    {
        // Simulate faster walking movement to cover more distance
        // Increased walking speed: ~15 km/h = ~4.2 m/s (like jogging/fast walking)
        // In 3 seconds: ~12.6 meters (3x the original distance)
        // GPS coordinates: 1 degree latitude ≈ 111,000 meters
        
        var random = new Random();
        
        // Generate a walking pattern with larger distances (could be straight, curved, or with turns)
        var walkingPatterns = new[]
        {
            // Straight line north (3x distance)
            (latChange: 0.000114, lonChange: 0.0),
            // Straight line east (3x distance)
            (latChange: 0.0, lonChange: 0.000114),
            // Diagonal northeast (3x distance)
            (latChange: 0.000081, lonChange: 0.000081),
            // Large curve (changing direction, 3x distance)
            (latChange: 0.000105 * Math.Sin(step * 0.3), lonChange: 0.000105 * Math.Cos(step * 0.3)),
            // Random walk with larger steps (3x distance)
            (latChange: (random.NextDouble() - 0.5) * 0.000228, lonChange: (random.NextDouble() - 0.5) * 0.000228)
        };
        
        // Choose pattern based on step (creates variety)
        var patternIndex = (step / 5) % walkingPatterns.Length;
        var pattern = walkingPatterns[patternIndex];
        
        var newLatitude = currentLocation.Latitude + pattern.latChange;
        var newLongitude = currentLocation.Longitude + pattern.lonChange;
        
        return new Location(newLatitude, newLongitude);
    }

    private void ProcessSimulatedLocation(Location location)
    {
        var locationText = $"{location.Latitude:F6}, {location.Longitude:F6}";
        var timestamp = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update current location display
            LocationLabel.Text = locationText;
            TimestampLabel.Text = $"Simulated: {timestamp}";
            
            // Update map
            UpdateMapLocation(location);
            
            // Add to history
            var locationItem = new LocationItem
            {
                LocationText = locationText,
                Timestamp = $"SIM {timestamp}",
                AccuracyText = "±2m",
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Accuracy = 2.0
            };
            
            LocationHistory.Insert(0, locationItem);
            
            // Add pin to map
            AddLocationPin(location, $"Step {_simulationStep}");
            
            // Keep history manageable
            while (LocationHistory.Count > 50)
            {
                LocationHistory.RemoveAt(LocationHistory.Count - 1);
            }
            
            // Update status
            var remainingTime = (_totalSimulationSteps - _simulationStep) * 3;
            UpdateStatus($"Walking simulation... {remainingTime}s remaining (Step {_simulationStep}/{_totalSimulationSteps})");
        });
        
        // Save simulated location to database
        _ = Task.Run(async () => await SaveLocationToDatabase(location, true));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_isTracking)
        {
            StopTracking();
        }
        if (_isSimulating)
        {
            StopWalkingSimulation();
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