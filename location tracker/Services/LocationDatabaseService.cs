using SQLite;
using LocationTracker.Models;

namespace LocationTracker.Services
{
    public class LocationDatabaseService
    {
        private SQLiteAsyncConnection? _database;

        public LocationDatabaseService()
        {
        }

        public async Task InitializeAsync()
        {
            if (_database is not null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "LocationTracker.db");
            _database = new SQLiteAsyncConnection(databasePath);
            
            await _database.CreateTableAsync<LocationRecord>();
        }

        public async Task<List<LocationRecord>> GetAllLocationsAsync()
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>()
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task<List<LocationRecord>> GetLocationsBySessionAsync(string sessionId)
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>()
                .Where(x => x.SessionId == sessionId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task<List<LocationRecord>> GetLocationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>()
                .Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task<List<LocationRecord>> GetRecentLocationsAsync(int count = 50)
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>()
                .OrderByDescending(x => x.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<LocationRecord> GetLocationByIdAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveLocationAsync(LocationRecord location)
        {
            await InitializeAsync();
            
            if (location.Id != 0)
            {
                return await _database.UpdateAsync(location);
            }
            else
            {
                return await _database.InsertAsync(location);
            }
        }

        public async Task<int> SaveLocationsAsync(List<LocationRecord> locations)
        {
            await InitializeAsync();
            return await _database.InsertAllAsync(locations);
        }

        public async Task<int> DeleteLocationAsync(LocationRecord location)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(location);
        }

        public async Task<int> DeleteLocationsBySessionAsync(string sessionId)
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>()
                .DeleteAsync(x => x.SessionId == sessionId);
        }

        public async Task<int> DeleteAllLocationsAsync()
        {
            await InitializeAsync();
            return await _database.DeleteAllAsync<LocationRecord>();
        }

        public async Task<int> GetLocationCountAsync()
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>().CountAsync();
        }

        public async Task<LocationRecord?> GetLastLocationAsync()
        {
            await InitializeAsync();
            return await _database.Table<LocationRecord>()
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetAllSessionIdsAsync()
        {
            await InitializeAsync();
            var allRecords = await _database!.Table<LocationRecord>()
                .Where(x => x.SessionId != "")
                .ToListAsync();
            
            return allRecords.Select(x => x.SessionId)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public async Task<double> CalculateTotalDistanceAsync(string sessionId)
        {
            await InitializeAsync();
            var locations = await GetLocationsBySessionAsync(sessionId);
            
            if (locations.Count < 2)
                return 0;

            double totalDistance = 0;
            for (int i = 1; i < locations.Count; i++)
            {
                var prev = locations[i - 1];
                var current = locations[i];
                
                totalDistance += CalculateDistance(prev.Latitude, prev.Longitude, current.Latitude, current.Longitude);
            }
            
            return totalDistance;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula for calculating distance between two GPS coordinates
            const double R = 6371000; // Earth's radius in meters
            
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return R * c; // Distance in meters
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public async Task<string> ExportToJsonAsync(string? sessionId = null)
        {
            await InitializeAsync();
            
            List<LocationRecord> locations;
            if (string.IsNullOrEmpty(sessionId))
            {
                locations = await GetAllLocationsAsync();
            }
            else
            {
                locations = await GetLocationsBySessionAsync(sessionId);
            }

            return System.Text.Json.JsonSerializer.Serialize(locations, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}