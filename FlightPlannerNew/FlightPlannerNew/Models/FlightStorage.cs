using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using FlightPlannerNew.Models;

namespace FlightPlannerNew.Models
{
    public class FlightStorage
    {
        public static int Id = 0;
        public static List<Flight> FlightsDataBase = new List<Flight>();

        public static void ClearFLights()
        {
            FlightsDataBase.Clear();
        }

        public static void AddFlights(Flight flights)
        {
            FlightsDataBase.Add(flights);
        }

        public static int GetRandomFlightId()
        {
            var randomNumber = new Random();
            var flightId = randomNumber.Next(0, 1000000);
            return flightId;
        }
    }
}