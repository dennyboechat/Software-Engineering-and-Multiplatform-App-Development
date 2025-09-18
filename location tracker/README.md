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
- Comprehensive location history with timestamps
- GPS accuracy information
- Coordinate display with precision
- Export-ready location data

### 🎯 **Advanced Tracking Features**
- Automatic location permission management
- Background location tracking capabilities
- Customizable tracking intervals
- Heat map visualization (temporarily disabled)

## 🛠️ **Technology Stack**

- **.NET 8.0** - Latest .NET framework
- **MAUI (Multi-platform App UI)** - Cross-platform development
- **Microsoft.Maui.Controls.Maps** - Interactive mapping
- **Microsoft.Maui.Essentials** - GPS and device services
- **SkiaSharp** - Custom graphics rendering
- **C# & XAML** - Modern UI development

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

## 🗂️ **Project Structure**

```
LocationTracker/
├── 📄 MainPage.xaml           # Main UI layout
├── 🔧 MainPage.xaml.cs        # Core application logic
├── 📄 App.xaml                # Application configuration
├── 🔧 MauiProgram.cs          # Application startup
├── 📄 AppShell.xaml           # Navigation shell
├── 📊 HeatMapData.cs          # Heat map data structure
├── 🎨 HeatMapOverlay.cs       # Custom heat map rendering
├── 📋 LocationTracker.csproj  # Project configuration
├── 📂 Platforms/              # Platform-specific code
│   └── 🍎 MacCatalyst/        # macOS-specific configuration
├── 📂 Resources/              # App resources
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

## 🛡️ **Privacy & Permissions**

### **Location Access**
- **When In Use**: Required for location tracking
- **Precise Location**: Needed for accurate GPS coordinates
- **Background Location**: Optional for continuous tracking

### **Data Handling**
- Location data stored locally only
- No external data transmission
- User controls all location sharing

## 🚧 **Development Status**

### ✅ **Completed Features**
- [x] Basic location services integration
- [x] Interactive map with pins
- [x] Real-time location tracking
- [x] Walking simulation with realistic patterns
- [x] Location history with timestamps
- [x] Permission management
- [x] MacCatalyst platform support

### 🔄 **In Progress**
- [ ] Heat map visualization (disabled due to stability)
- [ ] Multi-platform deployment (iOS, Android)

### 🎯 **Planned Features**
- [ ] Location data export (CSV, JSON)
- [ ] Route planning and navigation
- [ ] Geofencing capabilities
- [ ] Location sharing features

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