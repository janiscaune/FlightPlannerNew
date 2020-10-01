using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FlightPlannerNew.Models
{
    public class FlightsDataBaseContext : DbContext

    {
        public FlightsDataBaseContext() : base("FlightDatabase")
        {
            Database.SetInitializer<FlightsDataBaseContext>(null);
        }
        public  DbSet<Flight> FlightsDataBase { get; set; }

        public  DbSet<Airport> AirportDataBase { get; set; }
    }
}