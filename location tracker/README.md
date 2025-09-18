# 📍 Location Tracker

A modern .NET MAUI cross-platform application for tracking and visualizing GPS locations with interactive maps and walking simulation features.

## ✨ Features

### 🗺️ **Interactive Map Visualization**
- Real-time GPS location tracking
- Interactive street maps with user location display
- Pin markers showing location history
- Automatic map centering and zoom control

### 🚶‍♂️ **Walking Simulation**
- 1-minute walking simulation for testing
- Realistic GPS coordinate generation
- Multiple walking patterns (straight lines, curves, random walk)
- Step-by-step visualization on map

### 📊 **Location History & Data**
- **SQLite Database Storage** - Persistent location data across sessions
- **Session Management** - Group locations by tracking sessions
- **Comprehensive Metadata** - Timestamps, accuracy, speed, altitude
- **Data Analytics** - Distance calculations using Haversine formula
- **Export Functionality** - JSON data export capabilities
- **Coordinate Display** - Precise GPS coordinates with formatting

### 🎯 **Advanced Tracking Features**
- **Automatic Data Persistence** - All locations saved to SQLite database
- **Session-based Tracking** - Unique session IDs for each app launch
- **Error Recovery** - Graceful degradation when database unavailable
- **Real-time & Simulation Storage** - Both GPS and simulated data preserved
- **Automatic Location Permission Management** - Seamless permission handling
- **Background Location Tracking** - Continuous monitoring capabilities
- **Customizable Tracking Intervals** - Configurable update frequencies
- **Heat Map Visualization** - Advanced rendering (temporarily disabled)

## 🛠️ **Technology Stack**

### **Core Framework**
- **.NET 8.0** - Latest .NET framework with C# 12
- **MAUI (Multi-platform App UI)** - Cross-platform development
- **Microsoft.Maui.Controls.Maps** - Interactive mapping capabilities
- **Microsoft.Maui.Essentials** - GPS and device services integration

### **Database & Storage**
- **SQLite** - Local database for persistent location storage
- **sqlite-net-pcl** - .NET SQLite ORM for data operations
- **SQLitePCLRaw.bundle_green** - SQLite native bindings

### **UI & Graphics**
- **XAML** - Declarative UI markup language
- **SkiaSharp** - Custom graphics rendering engine
- **C# & MVVM** - Modern application architecture

## 🚀 **Getting Started**

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- macOS (for MacCatalyst deployment)
- Xcode command line tools

### Installation

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd "location tracker"
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run --framework net8.0-maccatalyst
   ```

## 📱 **Usage Guide**

### **Getting Your Location**
1. Launch the app
2. Grant location permissions when prompted
3. Tap **"Get My Location"** to fetch current GPS coordinates
4. View your location on the interactive map

### **Location Tracking**
1. Tap **"Start Tracking"** to begin continuous monitoring
2. Watch as location pins appear on the map
3. Check the location history for detailed tracking data
4. Tap **"Stop Tracking"** when finished

### **Walking Simulation** 🎯
1. Tap **"Simulate Walking"** (orange button)
2. Watch the 1-minute simulation showing realistic movement
3. Observe pins appearing every 3 seconds on the map
4. Stop early with **"Stop Simulation"** (red button) if needed
5. All simulated locations are automatically saved to database

### **Data Persistence** 💾
1. **Automatic Storage**: All GPS and simulated locations saved automatically
2. **Session Tracking**: Each app launch creates a new session ID
3. **History Loading**: Previous locations automatically restored on app start
4. **Cross-Session Data**: View location history from previous tracking sessions
5. **Export Data**: Use database service to export location data to JSON

## 🗂️ **Project Structure**

```
LocationTracker/
├── 📄 MainPage.xaml           # Main UI layout and controls
├── 🔧 MainPage.xaml.cs        # Core application logic & database integration
├── 📄 App.xaml                # Application configuration
├── 🔧 MauiProgram.cs          # Dependency injection & service registration
├── 📄 AppShell.xaml           # Navigation shell structure
├── 📂 Models/                 # Data models and entities
│   └── 📊 LocationRecord.cs   # SQLite entity for location data
├── 📂 Services/               # Business logic and data services  
│   └── 🗄️ LocationDatabaseService.cs # SQLite database operations
├── 📊 HeatMapData.cs          # Heat map data structure (legacy)
├── 🎨 HeatMapOverlay.cs       # Custom heat map rendering (disabled)
├── 📋 LocationTracker.csproj  # Project configuration & dependencies
├── 📂 Platforms/              # Platform-specific code
│   └── 🍎 MacCatalyst/        # macOS-specific configuration & entitlements
├── 📂 Resources/              # App resources and assets
│   ├── 🎨 AppIcon/            # Application icons
│   ├── 🎨 Images/             # Image assets
│   └── 🔤 Fonts/              # Custom fonts
└── 📄 README.md               # This file
```

## 🔧 **Configuration**

### **Location Permissions**
The app requires location permissions configured in:
- `Platforms/MacCatalyst/Info.plist`
- `Platforms/MacCatalyst/Entitlements.plist`

### **Map Settings**
- Default zoom: 200m radius for walking simulation
- Maximum pins displayed: 20 (walking trail visibility)
- Map type: Street view with user location

### **Simulation Parameters**
- Duration: 1 minute (20 steps × 3 seconds)
- Walking speed: ~5 km/h (realistic human pace)
- Distance per step: ~4.2 meters
- Update interval: 3 seconds

## �️ **Database Features**

### **SQLite Storage**
- **Location**: `[App Data Directory]/LocationTracker.db`
- **Automatic Creation**: Database and tables created on first run
- **Persistence**: Data survives app restarts and device reboots
- **Performance**: Optimized SQLite queries for smooth operation

### **Data Schema**
```sql
CREATE TABLE LocationRecords (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Latitude REAL NOT NULL,           -- GPS latitude coordinate
    Longitude REAL NOT NULL,          -- GPS longitude coordinate  
    Altitude REAL,                    -- Elevation above sea level
    Accuracy REAL,                    -- GPS accuracy in meters
    Speed REAL,                       -- Movement speed in m/s
    Heading REAL,                     -- Direction of movement
    Timestamp DATETIME NOT NULL,      -- When location was recorded
    TrackingType TEXT(20),            -- "GPS" or "SIM" for simulation
    SessionId TEXT(50),               -- Unique session identifier
    Notes TEXT(100),                  -- Additional metadata
    IsSimulated BOOLEAN               -- True for simulation data
);
```

### **Available Operations**
- **CRUD Operations**: Create, Read, Update, Delete location records
- **Session Queries**: Filter locations by tracking session
- **Date Range Filtering**: Query locations within time periods
- **Analytics**: Calculate total distance traveled per session
- **Data Export**: Export location data to JSON format
- **Recent Locations**: Quick access to latest tracked positions

### **Error Handling**
- **Graceful Degradation**: App continues working if database fails
- **Fallback Mode**: Memory-only operation when database unavailable
- **Silent Recovery**: Automatic error handling with debug logging
- **User Notifications**: Clear status messages about database state

## �🛡️ **Privacy & Permissions**

### **Location Access**
- **When In Use**: Required for location tracking
- **Precise Location**: Needed for accurate GPS coordinates
- **Background Location**: Optional for continuous tracking

### **Data Handling**
- **Local Storage Only**: All data stored in local SQLite database
- **No Cloud Sync**: No external data transmission or cloud storage
- **User Privacy**: Complete user control over location data
- **Data Ownership**: User owns all location data on their device
- **Offline Operation**: Full functionality without internet connection
- **Secure Storage**: Database stored in protected app data directory

## 🚧 **Development Status**

### ✅ **Completed Features**
- [x] **Core Location Services**: GPS tracking and location permissions
- [x] **Interactive Map Interface**: Street maps with pins and user location
- [x] **Real-time Location Tracking**: Continuous GPS monitoring with timers
- [x] **Walking Simulation**: Realistic 1-minute movement patterns for testing
- [x] **SQLite Database Integration**: Persistent storage for all location data
- [x] **Session Management**: Unique session IDs for grouping location data
- [x] **Data Analytics**: Distance calculations and location history analysis
- [x] **Error Recovery**: Graceful degradation and robust error handling
- [x] **Service Architecture**: Dependency injection and service locator patterns
- [x] MacCatalyst platform support

### 🔄 **In Progress**
- [ ] Heat map visualization (disabled due to stability)
- [ ] Multi-platform deployment (iOS, Android)

### 🎯 **Planned Features**
- [ ] CSV data export format
- [ ] Route planning and navigation
- [ ] Geofencing capabilities
- [ ] Location sharing features
- [ ] Cloud synchronization
- [ ] Advanced filtering and search

## 🐛 **Known Issues**

1. **Heat Map Crashes**: SkiaSharp heat map overlay temporarily disabled due to rendering conflicts
2. **Platform Limitations**: Currently optimized for MacCatalyst only
3. **OpenGLES Warning**: Framework compatibility warning (non-critical)

## 🤝 **Contributing**

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 **License**

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 **Troubleshooting**

### **App Won't Launch**
- Verify .NET 8.0 SDK installation
- Check Xcode command line tools
- Ensure location permissions granted

### **No Location Found**
- Check GPS/WiFi connectivity
- Verify location permissions in System Preferences
- Try the walking simulation for testing

### **Map Not Loading**
- Check internet connectivity
- Verify map API access
- Restart the application

## 📞 **Support**

For issues and questions:
- Check the troubleshooting section above
- Review known issues
- Submit bug reports with detailed information

---

**Made with ❤️ using .NET MAUI**

*Location Tracker v1.0 - Bringing GPS tracking to your fingertips*