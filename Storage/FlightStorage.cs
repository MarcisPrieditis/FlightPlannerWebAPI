using System;
using System.Collections.Generic;
using System.Linq;
using FlightPlanner.Models;

namespace FlightPlannerWebAPI.Storage
{
    public static class FlightStorage
    {
        private static List<Flight> _flights = new();
        private static int _id = 0;
        private static readonly object _locker = new();

        public static Flight AddFlight(AddFlightRequest request)
        {
            lock (_locker)
            {
                var flight = new Flight
                {
                    Id = ++_id,
                    From = request.From,
                    To = request.To,
                    Carrier = request.Carrier,
                    DepartureTime = request.DepartureTime,
                    ArrivalTime = request.ArrivalTime
                };
                _flights.Add(flight);

                return flight;
            }
        }

        public static Flight GetFlight(int id)
        {
            lock (_locker)
            {
                return _flights.SingleOrDefault(f => f.Id == id);
            }
        }

        public static void DeleteFlight(int id)
        {
            lock (_locker)
            {
                var flight = GetFlight(id);

                if (flight != null)
                    _flights.Remove(flight);
            }
        }

        public static List<Airport> FindAirports(string userInput)
        {
            lock (_locker)
            {
                userInput = userInput.ToLower().Trim();

                var fromAirport = _flights.Where(f => f.From.AirportName.ToLower().Trim().Contains(userInput) ||
                                                      f.From.City.ToLower().Trim().Contains(userInput) ||
                                                      f.From.Country.ToLower().Trim().Contains(userInput)).Select(f => f.From).ToList();

                var toAirport = _flights.Where(f => f.To.AirportName.ToLower().Trim().Contains(userInput) ||
                                                    f.To.City.ToLower().Trim().Contains(userInput) ||
                                                    f.To.Country.ToLower().Trim().Contains(userInput)).Select(f => f.To).ToList();

                return fromAirport.Concat(toAirport).ToList();
            }
        }

        public static void ClearFlights()
        {
            _flights.Clear();
            _id = 0;
        }

        public static bool CheckIfExist(AddFlightRequest flight)
        {
            lock (_locker)
            {
                return _flights.Any(f =>
                    f.From.AirportName.ToLower().Trim() == flight.From.AirportName.ToLower().Trim() &&
                    f.To.AirportName.ToLower().Trim() == flight.To.AirportName.ToLower().Trim() &&
                    f.Carrier.ToLower().Trim() == flight.Carrier.ToLower().Trim() &&
                    f.DepartureTime == flight.DepartureTime &&
                    f.ArrivalTime == flight.ArrivalTime);
            }
        }

        public static bool CheckWrongValues(AddFlightRequest flight)
        {
            lock (_locker)
            {
                if (flight == null || flight.To == null || flight.From == null ||
                    string.IsNullOrEmpty(flight.Carrier) || string.IsNullOrEmpty(flight.DepartureTime) || string.IsNullOrEmpty(flight.ArrivalTime) ||
                    string.IsNullOrEmpty(flight.To.AirportName) || string.IsNullOrEmpty(flight.From.AirportName))
                    return true;

                //Make sure there are no identical airports
                if (flight.From.AirportName.ToLower().Trim() == flight.To.AirportName.ToLower().Trim())
                    return true;

                var arrivalTime = DateTime.Parse(flight.ArrivalTime);
                var departureTime = DateTime.Parse(flight.DepartureTime);
                if (arrivalTime <= departureTime)
                    return true;

                return false;
            }
        }

        public static bool InvalidFlightValues(SearchFlightRequest flight)
        {
            lock (_locker)
            {
                return string.IsNullOrEmpty(flight.To) || string.IsNullOrEmpty(flight.From) || string.IsNullOrEmpty(flight.DepartureDate) ||
                       flight.From.ToLower().Trim() == flight.To.ToLower().Trim();
            }
        }

        public static PageResult SearchFlight()
        {
            lock (_locker)
            {
                return new PageResult(_flights);
            }
        }
    }
}