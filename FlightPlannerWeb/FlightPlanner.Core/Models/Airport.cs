using System.Text.Json.Serialization;
using FlightPlanner.Core.Models;

namespace FlightPlanner.Models
{
    public class Airport : Entity
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string AirportName { get; set; }
    }
}