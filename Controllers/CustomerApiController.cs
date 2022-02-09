using FlightPlanner.Models;
using FlightPlannerWebAPI.Storage;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlannerWebAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private static readonly object _locker = new();

        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {

            var airports = FlightStorage.FindAirports(search);

            return Ok(airports);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchFlightRequest search)
        {
            if (FlightStorage.InvalidFlightValues(search))
                return BadRequest();

            return Ok(FlightStorage.SearchFlight());
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult SearchFlightById(int id)
        {
            var flight = FlightStorage.GetFlight(id);

            if (flight == null)
                return NotFound();

            return Ok(flight);
        }
    }
}
