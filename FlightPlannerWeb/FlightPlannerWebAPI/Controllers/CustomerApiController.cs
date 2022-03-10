using System.Linq;
using FlightPlanner.Models;
using FlightPlannerWebAPI.Storage;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FlightPlannerWebAPI.Controllers
{
    [Route("api")]
    [EnableCors]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private static readonly object _locker = new();
        private readonly FlightPlannerDbContext _context;

        public CustomerApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {
            var airports = FlightStorage.FindAirports(search, _context);

            return Ok(airports);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchFlightRequest search)
        {
            if (FlightStorage.InvalidFlightValues(search))
                return BadRequest();

            return Ok(FlightStorage.SearchFlight(search, _context));
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult SearchFlightById(int id)
        {
            var flight = _context.Flights
                .Include(f => f.From)
                .Include(f => f.To)
                .SingleOrDefault(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }
    }
}
