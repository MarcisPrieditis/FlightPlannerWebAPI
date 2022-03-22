using System;
using System.Collections.Generic;
using System.Linq;
using FlightPlanner.Core.Models;
using FlightPlanner.Data;
using FlightPlanner.Models;

namespace FlightPlannerWebAPI.Storage
{
    public static class FlightStorage
    {
        private static readonly object _locker = new();

        public static Flight ConvertToFlight(AddFlightRequest request)
        {
            var flight = new Flight
            {
                From = request.From,
                To = request.To,
                Carrier = request.Carrier,
                DepartureTime = request.DepartureTime,
                ArrivalTime = request.ArrivalTime
            };
            return flight;
        }

        public static List<Airport> FindAirports(string userInput, FlightPlannerDbContext context)
        {
            lock (_locker)
            {
                userInput = userInput.ToLower().Trim();

                var fromAirport = context.Flights.Where(f => f.From.AirportName.ToLower().Trim().Contains(userInput) ||
                                                             f.From.City.ToLower().Trim().Contains(userInput) ||
                                                             f.From.Country.ToLower().Trim().Contains(userInput)).Select(f => f.From).ToList();

                var toAirport = context.Flights.Where(f => f.To.AirportName.ToLower().Trim().Contains(userInput) ||
                                                           f.To.City.ToLower().Trim().Contains(userInput) ||
                                                           f.To.Country.ToLower().Trim().Contains(userInput)).Select(f => f.To).ToList();

                return fromAirport.Concat(toAirport).ToList();
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

        public static PageResult SearchFlight(SearchFlightRequest request, FlightPlannerDbContext context)
        {
            lock (_locker)
            {
                var search = context.Flights.Where(f =>
                    f.From.AirportName.ToLower().Trim() == request.From.ToLower().Trim() &&
                    f.To.AirportName.ToLower().Trim() == request.To.ToLower().Trim() &&
                    f.DepartureTime.Substring(0, 10) == request.DepartureDate).ToList();

                return new PageResult(search);
            }
        }
    }
}

