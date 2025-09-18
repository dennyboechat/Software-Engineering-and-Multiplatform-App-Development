using Microsoft.Maui.Maps;
using SkiaSharp;

namespace LocationTracker;

public class HeatMapPoint
{
    public Location Location { get; set; }
    public double Weight { get; set; } = 1.0;
    public DateTime Timestamp { get; set; }
    
    public HeatMapPoint(Location location, double weight = 1.0)
    {
        Location = location;
        Weight = weight;
        Timestamp = DateTime.Now;
    }
}

public class HeatMapData
{
    public List<HeatMapPoint> Points { get; set; } = new();
    public double MaxWeight { get; set; } = 1.0;
    public double MinWeight { get; set; } = 0.1;
    
    public void AddPoint(Location location, double weight = 1.0)
    {
        // Check if there's already a point very close to this location
        var existingPoint = Points.FirstOrDefault(p => 
            CalculateDistance(p.Location, location) < 0.0001); // ~10 meters
            
        if (existingPoint != null)
        {
            // Increase weight of existing point
            existingPoint.Weight += weight;
            existingPoint.Timestamp = DateTime.Now;
        }
        else
        {
            // Add new point
            Points.Add(new HeatMapPoint(location, weight));
        }
        
        // Update max weight
        MaxWeight = Math.Max(MaxWeight, Points.Max(p => p.Weight));
    }
    
    public void ClearPoints()
    {
        Points.Clear();
        MaxWeight = 1.0;
    }
    
    private double CalculateDistance(Location loc1, Location loc2)
    {
        return Math.Sqrt(Math.Pow(loc1.Latitude - loc2.Latitude, 2) + 
                        Math.Pow(loc1.Longitude - loc2.Longitude, 2));
    }
}