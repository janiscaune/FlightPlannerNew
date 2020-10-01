using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        
        private static object myLockObject = new object();

        //public static List<Flight> GetToList()
        //{
        //    lock (myLockObject)
        //    {
        //        var flightsDataBaseList = FlightsDataBase.ToList();
        //        return flightsDataBaseList;
        //    }
        //}

        public static void ClearFLights()
        {
            lock (myLockObject)
            {
                using (var context = new FlightsDataBaseContext())
                {
                    context.AirportDataBase.RemoveRange(context.AirportDataBase);
                    context.FlightsDataBase.RemoveRange(context.FlightsDataBase);
                    context.SaveChanges();
                }
            }
        }

        public static HttpResponseMessage AddFlights(HttpRequestMessage message, Flight flights)
        {

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

            //if (IsSameFLight(flights))
            //{
            //    return message.CreateResponse(HttpStatusCode.Conflict);
            //}
            
            lock (myLockObject)
            {
                if (IsSameFLight(flights))
                {
                    return message.CreateResponse(HttpStatusCode.Conflict);
                }
            }

            lock (myLockObject)
            {
                using (var context = new FlightsDataBaseContext())
                {
                 flights = context.FlightsDataBase.Add(flights);
                 context.SaveChanges();

                 var newFlight = new FlightWithoutId()
                 {
                     Id = flights.Id, 
                     From = new AirportWithoutId()
                     {
                         City = flights.From.City, Country = flights.From.Country, airport = flights.From.airport

                     },
                     To = new  AirportWithoutId()
                     {
                         City = flights.To.City, Country =  flights.To.Country, airport = flights.To.airport
                     },
                     Carrier = flights.Carrier,
                     DepartureTime = flights.DepartureTime,
                     ArrivalTime = flights.ArrivalTime
                 };
                 return message.CreateResponse(HttpStatusCode.Created, newFlight);
                }
            }
        }

        public static bool IsSameFLight(Flight flight)
        {
            using (var context = new FlightsDataBaseContext())
            {
                return context.FlightsDataBase.Include(f => f.From).Include(f => f.To).ToList().Any(f => f.ArrivalTime == flight.ArrivalTime &&
                                                    f.Carrier == flight.Carrier &&
                                                    f.DepartureTime == flight.DepartureTime &&
                                                    f.From.City == flight.From.City &&
                                                    f.From.Country == flight.From.Country &&
                                                    f.From.airport == flight.From.airport &&
                                                    f.To.City == flight.To.City &&
                                                    f.To.Country == flight.To.Country &&
                                                    f.To.airport == flight.To.airport);
            }
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
        public static HttpResponseMessage Delete(HttpRequestMessage message, int id)
        {
            lock (myLockObject)
            {
                using (var context = new FlightsDataBaseContext())
                {
                    if (!context.FlightsDataBase.Any(f => f.Id == id))
                    {
                        return message.CreateResponse(HttpStatusCode.OK);
                    }
                    var flight = context.FlightsDataBase.Include(f => f.From).Include(f => f.To)
                        .Single(f => f.Id == id);
                    context.FlightsDataBase.Remove(flight);
                    context.SaveChanges();
                    return message.CreateResponse(HttpStatusCode.OK);
                }
            }
        }
        public static HttpResponseMessage GetTheAirport(HttpRequestMessage message, string searchAirport)
        {
            using (var context = new FlightsDataBaseContext())
            {
                var lookForAirport = context.AirportDataBase.Where(f =>
                        f.City.ToLower().Contains(searchAirport.ToLower().Trim()) ||
                        f.Country.ToLower().Contains(searchAirport.ToLower().Trim()) ||
                        f.airport.ToLower().Contains(searchAirport.ToLower().Trim())).ToList()
                    .Select(f => new AirportWithoutId() {City = f.City, Country = f.Country, airport = f.airport});
                return message.CreateResponse(HttpStatusCode.OK, lookForAirport);
            }
            
            //var airportList = new List<Airport>();

            //foreach (var flight in lookForAirport)
            //{
            //    airportList.Add(new Airport(flight.From.Country, flight.From.City, flight.From.airport));
            //}

        }
        public static HttpResponseMessage SearchForAirport(HttpRequestMessage message, SearchFlightRequest searchFlight)
        {
            using (var context = new FlightsDataBaseContext())
            {
                if (searchFlight.From == searchFlight.To)
                {
                    return message.CreateResponse(HttpStatusCode.BadRequest);
                }
                var page = 0;
                var items = context.FlightsDataBase.Include(f => f.From).Include(f => f.To)
                    .AsEnumerable().Where(f => f.From.airport == searchFlight.From &&
                                                           f.To.airport == searchFlight.To &&
                                                           $"{DateTime.Parse(f.DepartureTime):yyyy-MM-dd}" == searchFlight.DepartureDate).ToList();

                var totalItems = items.Count();

                var pageResult = new PageResult(page, totalItems, items);

                return message.CreateResponse(HttpStatusCode.OK, pageResult);
            }
        }
        public static HttpResponseMessage SearchFlightById(HttpRequestMessage message, int id)
        {
            using (var context = new FlightsDataBaseContext())
            {

                if (!context.FlightsDataBase.Any(f => f.Id == id))
                {
                    return message.CreateResponse(HttpStatusCode.NotFound);
                }
                var findFlightById = context.FlightsDataBase.Include(f => f.From).Include(f => f.To).SingleOrDefault(f => f.Id == id);

                var newFlight = new FlightWithoutId()
                {
                    Id = findFlightById.Id,
                    From = new AirportWithoutId()
                    {
                        City = findFlightById.From.City,
                        Country = findFlightById.From.Country,
                        airport = findFlightById.From.airport

                    },
                    To = new AirportWithoutId()
                    {
                        City = findFlightById.To.City,
                        Country = findFlightById.To.Country,
                        airport = findFlightById.To.airport
                    },
                    Carrier = findFlightById.Carrier,
                    DepartureTime = findFlightById.DepartureTime,
                    ArrivalTime = findFlightById.ArrivalTime
                };
                return message.CreateResponse(HttpStatusCode.OK, newFlight);
            }
        }
    }
}