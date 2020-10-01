using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http.ModelBinding;
using FlightPlannerNew.Models;

namespace FlightPlannerNew.Models
{
    public class FlightStorage
    {
        public static int Id = 0;
        private static object myLockObject = new object();

        public static List<Flight> FlightsDataBase = new List<Flight>();

        public static List<Flight> GetToList()
        {
            lock (myLockObject)
            {
                var flightsDataBaseList = FlightsDataBase.ToList();
                return flightsDataBaseList;
            }
        }

        public static void ClearFLights()
        {
            lock (myLockObject)
            {
                FlightsDataBase.Clear();
            }
        }

        public static HttpResponseMessage AddFlights(HttpRequestMessage message, Flight flights)
        {
            lock (myLockObject)
            {
                foreach (var trip in FlightsDataBase)
                {
                    if (trip.Id == flights.Id)
                    {
                        flights.Id = GetRandomFlightId();
                    }
                }
            }

            if (IsInvalidValue(flights))
            {
                return message.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (IsSameAirport(flights))
            {
                return message.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (IsStrangeDates(flights))
            {
                return message.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (FlightsDataBase.Count > 0)
            {
                if (IsSameFLight(flights))
                {
                    return message.CreateResponse(HttpStatusCode.Conflict);
                }
            }

            lock (myLockObject)
            {
                if (IsSameFLight(flights))
                {
                    return message.CreateResponse(HttpStatusCode.Conflict);
                }
                FlightsDataBase.Add(flights);
            }
            return message.CreateResponse(HttpStatusCode.Created, flights);
        }

        public static bool IsSameFLight(Flight flight)
        {
            var flightsDataBaseList = GetToList();
            
                return flightsDataBaseList.Any(f => f.ArrivalTime == flight.ArrivalTime &&
                                                    f.Carrier == flight.Carrier &&
                                                    f.DepartureTime == flight.DepartureTime &&
                                                    f.From.City == flight.From.City &&
                                                    f.From.Country == flight.From.Country &&
                                                    f.From.airport == flight.From.airport &&
                                                    f.To.City == flight.To.City &&
                                                    f.To.Country == flight.To.Country &&
                                                    f.To.airport == flight.To.airport);
                
            
        }

        public static bool IsInvalidValue(Flight flight)
        {
            return
                flight.From == null || flight.To == null ||
                string.IsNullOrEmpty(flight.ArrivalTime) ||
                string.IsNullOrEmpty(flight.DepartureTime) ||
                string.IsNullOrEmpty(flight.Carrier) ||
                string.IsNullOrEmpty(flight.From.City) ||
                string.IsNullOrEmpty(flight.From.Country) ||
                string.IsNullOrEmpty(flight.From.airport) ||
                string.IsNullOrEmpty(flight.To.City) ||
                string.IsNullOrEmpty(flight.To.Country) ||
                string.IsNullOrEmpty(flight.To.airport);

        }

        public static bool IsSameAirport(Flight flight)
        {
            return flight.From.airport.ToLower().Trim() == flight.To.airport.ToLower().Trim();
        }

        public static bool IsStrangeDates(Flight flight)
        {
            var departureTime = DateTime.Parse(flight.DepartureTime);
            var arrivalTime = DateTime.Parse(flight.ArrivalTime);

            if (departureTime >= arrivalTime)
            {
                return true;
            }

            return false;
        }

        public static int GetRandomFlightId()
        {
            var randomNumber = new Random();
            var flightId = randomNumber.Next(0, 1000000);
            return flightId;
        }

        public static HttpResponseMessage Delete(HttpRequestMessage message, int id)
        {
            lock (myLockObject)
            {
                if (FlightsDataBase.Exists(f => f.Id == id))
                {
                    var findFlight = FlightsDataBase.FindIndex(f => f.Id == id);
                    FlightsDataBase.RemoveAt(findFlight);
                }
            }
            return message.CreateResponse(HttpStatusCode.OK);
        }

        public static HttpResponseMessage GetTheAirport(HttpRequestMessage message, string searchAirport)
        {
            var flightsDataBaseList = GetToList();
            
                if (flightsDataBaseList == null)
                {
                    return message.CreateResponse(HttpStatusCode.NotFound);
                }

                var lookForAirport = flightsDataBaseList.Where(f =>
                        f.From.City.ToLower().Contains(searchAirport.ToLower().Trim()) ||
                        f.From.Country.ToLower().Contains(searchAirport.ToLower().Trim()) ||
                        f.From.airport.ToLower().Contains(searchAirport.ToLower().Trim()))
                    .Select(f => new Airport(f.From.Country, f.From.City, f.From.airport));

                return message.CreateResponse(HttpStatusCode.OK, lookForAirport);
            
            //var airportList = new List<Airport>();

            //foreach (var flight in lookForAirport)
            //{
            //    airportList.Add(new Airport(flight.From.Country, flight.From.City, flight.From.airport));
            //}

        }

        public static HttpResponseMessage SearchForAirport(HttpRequestMessage message, SearchFlightRequest searchFlight)
        {
            if (searchFlight.From == searchFlight.To)
            {
                return message.CreateResponse(HttpStatusCode.BadRequest);
            }

            var flightsDataBaseList = GetToList();
            var page = 0;
            var items = flightsDataBaseList.Where(f => f.From.airport == searchFlight.From &&
                                                     f.To.airport == searchFlight.To &&
                                                     $"{DateTime.Parse(f.DepartureTime):yyyy-MM-dd}" == searchFlight.DepartureDate).ToList();

            var  totalItems = items.Count();

            var pageResult = new PageResult(page, totalItems, items);

            return message.CreateResponse(HttpStatusCode.OK, pageResult);
            }
        
        public static HttpResponseMessage SearchFlightById(HttpRequestMessage message, int id)
        {
            var flightsDataBaseList = GetToList();

                if (!flightsDataBaseList.Exists(f => f.Id == id))
                {
                    return message.CreateResponse(HttpStatusCode.NotFound);
                }
                var findFlightById = flightsDataBaseList.SingleOrDefault(f => f.Id == id);
                return message.CreateResponse(HttpStatusCode.OK, findFlightById);
            }

    }
}