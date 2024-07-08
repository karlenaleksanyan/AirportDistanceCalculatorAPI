using AirportDistanceCalculatorAPI.Models;
using AirportDistanceCalculatorAPI.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AirportDistanceCalculatorAPI.Services
{
    public class AirportService : IAirportService
    {
        private readonly HttpClient _httpClient;
        //Radius of the Earth in kilometers
        private readonly double r = 6371;

        public AirportService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<double> GetDistanceBetweenAirportsAsync(string iataCode1, string iataCode2)
        {
            var airport1 = await GetAirportDataAsync(iataCode1);
            var airport2 = await GetAirportDataAsync(iataCode2);

            if (airport1 == null || airport2 == null)
            {
                throw new Exception("One or both of the IATA codes are invalid.");
            }

            var distance = CalculateDistance(airport1.Latitude, airport1.Longitude, airport2.Latitude, airport2.Longitude);
            return distance;
        }

        private async Task<Airport> GetAirportDataAsync(string iataCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.api-ninjas.com/v1/airports?iata={iataCode}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception($"Airport data not found for IATA code: {iataCode}");
                    }
                    else
                    {
                        throw new Exception($"Error retrieving data for IATA code: {iataCode}. Status code: {response.StatusCode}");
                    }
                }

                var content = await response.Content.ReadAsStringAsync();
                var airportData = JArray.Parse(content).FirstOrDefault() as JObject;

                if (airportData == null)
                {
                    return new Airport();
                }

                return new Airport
                {
                    IataCode = iataCode,
                    Latitude = (double)airportData["latitude"],
                    Longitude = (double)airportData["longitude"]
                };
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error retrieving airport data: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing airport data: {ex.Message}");
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = r * c;
            return distance;
        }

        private double ToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }
    }
}
