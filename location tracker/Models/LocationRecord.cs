using SQLite;

namespace LocationTracker.Models
{
    [Table("LocationRecords")]
    public class LocationRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public double Latitude { get; set; }

        [NotNull]
        public double Longitude { get; set; }

        public double? Altitude { get; set; }

        public double? Accuracy { get; set; }

        public double? Speed { get; set; }

        public double? Heading { get; set; }

        [NotNull]
        public DateTime Timestamp { get; set; }

        [MaxLength(20)]
        public string TrackingType { get; set; } = "GPS"; // "GPS", "SIM" for simulation

        [MaxLength(50)]
        public string SessionId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Notes { get; set; } = string.Empty;

        public bool IsSimulated { get; set; } = false;

        // Computed properties for display
        [Ignore]
        public string LocationText => $"{Latitude:F6}, {Longitude:F6}";

        [Ignore]
        public string TimestampText => Timestamp.ToString("HH:mm:ss dd/MM/yyyy");

        [Ignore]
        public string AccuracyText => Accuracy.HasValue ? $"±{Accuracy:F0}m" : "N/A";

        [Ignore]
        public string DisplayType => IsSimulated ? "SIM" : "GPS";
    }
}