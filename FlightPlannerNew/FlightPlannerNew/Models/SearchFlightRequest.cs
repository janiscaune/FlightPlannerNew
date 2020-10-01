using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightPlannerNew.Models
{
    public class SearchFlightRequest
    {
        public string From { get; set; }

        public string To { get; set; }

        public string DepartureDate { get; set; }

        public SearchFlightRequest(string from, string to, string departureDate)
        {
            this.From = from;

            this.To = to;

            this.DepartureDate = departureDate;

        }
    }
}