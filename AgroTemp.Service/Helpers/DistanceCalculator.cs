using System;
using System.Globalization;
using System.Text.Json;

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

        public static async Task<double?> GetRoadRouteDistanceInKilometersAsync(
            decimal lat1,
            decimal lon1,
            decimal lat2,
            decimal lon2,
            HttpClient? httpClient = null,
            CancellationToken cancellationToken = default)
        {
            var ownsClient = httpClient == null;
            httpClient ??= new HttpClient();

            if (ownsClient)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(8);
            }

            try
            {
                var url = $"https://router.project-osrm.org/route/v1/driving/" +
                          $"{lon1.ToString(CultureInfo.InvariantCulture)},{lat1.ToString(CultureInfo.InvariantCulture)};" +
                          $"{lon2.ToString(CultureInfo.InvariantCulture)},{lat2.ToString(CultureInfo.InvariantCulture)}" +
                          "?overview=false&alternatives=false&steps=false";

                using var response = await httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
                var root = document.RootElement;

                if (!root.TryGetProperty("code", out var code) || !string.Equals(code.GetString(), "Ok", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                if (!root.TryGetProperty("routes", out var routes) || routes.ValueKind != JsonValueKind.Array || routes.GetArrayLength() == 0)
                {
                    return null;
                }

                var firstRoute = routes[0];
                if (!firstRoute.TryGetProperty("distance", out var distanceMetersElement))
                {
                    return null;
                }

                var distanceMeters = distanceMetersElement.GetDouble();
                return distanceMeters / 1000d;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (ownsClient)
                {
                    httpClient.Dispose();
                }
            }
        }
    }
}
