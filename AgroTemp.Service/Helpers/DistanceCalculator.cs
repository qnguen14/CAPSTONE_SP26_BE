using System;

namespace AgroTemp.Service.Helpers
{
    public static class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371; // Earth's radius in kilometers

        public static double GetDistanceInKilometers(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var lat1Rad = DegreesToRadians((double)lat1);
            var lon1Rad = DegreesToRadians((double)lon1);
            var lat2Rad = DegreesToRadians((double)lat2);
            var lon2Rad = DegreesToRadians((double)lon2);

            var dLat = lat2Rad - lat1Rad;
            var dLon = lon2Rad - lon1Rad;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusKm * c;

            return distance;
        }

        // Convert degrees to radians
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static string GetDistanceDisplay(double distanceKm)
        {
            if (distanceKm < 1)
            {
                return $"{Math.Round(distanceKm * 1000)} m";
            }
            return $"{Math.Round(distanceKm, 1)} km";
        }
    }
}
