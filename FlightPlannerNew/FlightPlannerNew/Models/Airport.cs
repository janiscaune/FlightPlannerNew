using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FlightPlannerNew.Models
{
    public class Airport
    {
        public string Country { get; set; }
        public string City { get; set; }
        [JsonProperty("airport")] 
        public string airport { get; set; }

    }
}