using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FlightPlannerNew.Models
{
    public class AirportWithoutId
    {
        public string Country { get; set; }
        public string City { get; set; }
        [JsonProperty(PropertyName = "airport")]
        public string airport { get; set; }

    }
}