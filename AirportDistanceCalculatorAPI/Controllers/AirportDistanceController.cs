using AirportDistanceCalculatorAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirportDistanceCalculatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportDistanceController : ControllerBase
    {
        private readonly IAirportService _airportService;

        public AirportDistanceController(
            IAirportService airportService)
        {
            _airportService = airportService;
        }

        /// <summary>
        /// Calculates the distance between two airports.
        /// </summary>
        /// <param name="iataCode1">IATA code of the first airport.</param>
        /// <param name="iataCode2">IATA code of the second airport.</param>
        /// <returns>The distance between the two airports in kilometers.</returns>
        [HttpGet("{iataCode1}/{iataCode2}")]
        public async Task<ActionResult<double>> GetDistance(string iataCode1, string iataCode2)
        {
            var distance = await _airportService.GetDistanceBetweenAirportsAsync(iataCode1, iataCode2);
            return Ok(distance);
        }
    }

}
