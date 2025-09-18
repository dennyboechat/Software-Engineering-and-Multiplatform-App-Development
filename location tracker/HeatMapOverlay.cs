using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Maps;

namespace LocationTracker;

public class HeatMapOverlay : SKCanvasView
{
    private HeatMapData _heatMapData = new();
    private MapSpan? _mapSpan;
    
    public static readonly BindableProperty HeatMapDataProperty = BindableProperty.Create(
        nameof(HeatMapData), typeof(HeatMapData), typeof(HeatMapOverlay), 
        propertyChanged: OnHeatMapDataChanged);
        
    public static readonly BindableProperty MapSpanProperty = BindableProperty.Create(
        nameof(MapSpan), typeof(MapSpan), typeof(HeatMapOverlay),
        propertyChanged: OnMapSpanChanged);
    
    public HeatMapData HeatMapData
    {
        get => (HeatMapData)GetValue(HeatMapDataProperty);
        set => SetValue(HeatMapDataProperty, value);
    }
    
    public MapSpan MapSpan
    {
        get => (MapSpan)GetValue(MapSpanProperty);
        set => SetValue(MapSpanProperty, value);
    }
    
    private static void OnHeatMapDataChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is HeatMapOverlay overlay)
        {
            overlay._heatMapData = newValue as HeatMapData ?? new HeatMapData();
            overlay.InvalidateSurface();
        }
    }
    
    private static void OnMapSpanChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is HeatMapOverlay overlay)
        {
            overlay._mapSpan = newValue as MapSpan;
            overlay.InvalidateSurface();
        }
    }
    
    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        
        if (_heatMapData?.Points == null || !_heatMapData.Points.Any() || _mapSpan == null)
            return;
            
        var info = e.Info;
        DrawHeatMap(canvas, info);
    }
    
    private void DrawHeatMap(SKCanvas canvas, SKImageInfo info)
    {
        var radius = 30f;
        var width = info.Width;
        var height = info.Height;
        
        // Create a bitmap for the heat map
        using var bitmap = new SKBitmap(width, height);
        using var bitmapCanvas = new SKCanvas(bitmap);
        
        foreach (var point in _heatMapData.Points)
        {
            // Convert lat/lng to screen coordinates
            var screenPoint = LocationToScreen(point.Location, _mapSpan!, width, height);
            
            if (screenPoint.X < -radius || screenPoint.X > width + radius ||
                screenPoint.Y < -radius || screenPoint.Y > height + radius)
                continue;
                
            // Calculate intensity based on weight
            var intensity = (float)(point.Weight / _heatMapData.MaxWeight);
            
            // Create radial gradient for heat point
            using var paint = new SKPaint();
            var colors = new SKColor[]
            {
                SKColor.Parse("#FF0000").WithAlpha((byte)(255 * intensity)), // Red center
                SKColor.Parse("#FFFF00").WithAlpha((byte)(200 * intensity)), // Yellow
                SKColor.Parse("#00FF00").WithAlpha((byte)(150 * intensity)), // Green
                SKColors.Transparent // Transparent edge
            };
            
            var positions = new float[] { 0f, 0.3f, 0.7f, 1f };
            
            paint.Shader = SKShader.CreateRadialGradient(
                screenPoint, radius, colors, positions, SKShaderTileMode.Clamp);
            paint.IsAntialias = true;
            paint.BlendMode = SKBlendMode.Plus;
            
            bitmapCanvas.DrawCircle(screenPoint, radius, paint);
        }
        
        // Draw the heat map bitmap onto the main canvas with some transparency
        using var finalPaint = new SKPaint { Color = SKColors.White.WithAlpha(200) };
        canvas.DrawBitmap(bitmap, 0, 0, finalPaint);
    }
    
    private SKPoint LocationToScreen(Location location, MapSpan mapSpan, int width, int height)
    {
        var center = mapSpan.Center;
        var latDelta = mapSpan.LatitudeDegrees;
        var lonDelta = mapSpan.LongitudeDegrees;
        
        // Convert geographic coordinates to normalized coordinates (0-1)
        var normalizedX = (location.Longitude - (center.Longitude - lonDelta / 2)) / lonDelta;
        var normalizedY = 1 - (location.Latitude - (center.Latitude - latDelta / 2)) / latDelta;
        
        // Convert to screen coordinates
        var x = (float)(normalizedX * width);
        var y = (float)(normalizedY * height);
        
        return new SKPoint(x, y);
    }
}