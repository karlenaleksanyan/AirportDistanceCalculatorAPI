using System.Threading.Tasks;

namespace AirportDistanceCalculatorAPI.Services.Interfaces
{
    public interface IAirportService
    {
        Task<double> GetDistanceBetweenAirportsAsync(string iataCode1, string iataCode2);
    }
}
