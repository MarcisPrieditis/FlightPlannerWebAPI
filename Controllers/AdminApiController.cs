using System.Threading.Tasks;
using FlightPlanner.Models;
using FlightPlannerWebAPI.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlannerWebAPI.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private static readonly object _locker = new object();

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            var flight = FlightStorage.GetFlight(id);
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
                if (FlightStorage.CheckIfExist(request))
                    return Conflict(); //409

                if (FlightStorage.CheckWrongValues(request))
                    return BadRequest(); //400

                return Created("", FlightStorage.AddFlight(request));
            }
        }

        [HttpDelete]
        [Route("flights/{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(int id)
        {
            lock (_locker)
            {
                FlightStorage.DeleteFlight(id);
                return Ok();
            }
        }
    }
}

