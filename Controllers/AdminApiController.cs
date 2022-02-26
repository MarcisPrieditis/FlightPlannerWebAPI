using System.Linq;
using FlightPlanner.Models;
using FlightPlannerWebAPI.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlannerWebAPI.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [EnableCors]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _locker = new object();

        public AdminApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            var flight = _context.Flights
                    .Include(f => f.From)
                    .Include(f => f.To)
                    .SingleOrDefault(f => f.Id == id);
            //FlightStorage.GetFlight(id);
            if (flight == null)
                return NotFound();

            return Ok(flight);
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult AddFlights(AddFlightRequest request)
        {
            lock (_locker)
            {
                if (FLightExists(request))
                    return Conflict(); //409

                if (FlightStorage.CheckWrongValues(request))
                    return BadRequest(); //400

                var flight = FlightStorage.ConvertToFlight(request);
                _context.Flights.Add(flight);
                _context.SaveChanges();

                return Created("", flight);
            }
        }

        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            lock (_locker)
            {
                var flight = _context.Flights
                    .Include(f => f.From)
                    .Include(f => f.To)
                    .SingleOrDefault(f => f.Id == id);

                if (flight != null)
                {
                    _context.Flights.Remove(flight);
                    _context.SaveChanges();
                }

                return Ok();
            }
        }

        private bool FLightExists(AddFlightRequest request)
        {
            lock (_locker)
            {
                return Enumerable.Any(_context.Flights
                        .Include(f => f.To)
                        .Include(f => f.From),
                    flight => flight.ArrivalTime == request.ArrivalTime &&
                              flight.Carrier == request.Carrier && flight.DepartureTime == request.DepartureTime &&
                              flight.From.AirportName.Trim().ToLower() == request.From.AirportName.Trim().ToLower() &&
                              flight.To.AirportName.Trim().ToLower() == request.To.AirportName.Trim().ToLower());
            }
        }
    }
}

