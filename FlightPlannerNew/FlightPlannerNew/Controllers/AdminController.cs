using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightPlannerNew.Attributes;
using FlightPlannerNew.Models;


namespace FlightPlannerNew.Controllers
{
    [BasicAuthentication, Route("admin-api")]
    public class AdminController : ApiController
    {
        // GET: api/Admin
        [HttpGet, Route("admin-api/flights/{id}")]
        public HttpResponseMessage Flights(HttpRequestMessage message, int id)
        {
            return FlightStorage.SearchFlightById(message, id);
        }

        [HttpPut, Route("admin-api/flights")]
        // PUT: api/Admin/5
        public HttpResponseMessage Put(HttpRequestMessage message, Flight flight)
        {
            return FlightStorage.AddFlights(message, flight);
        }

        // DELETE: api/Admin/5
        [HttpDelete, Route("admin-api/flights/{id}")]
        public HttpResponseMessage Delete(HttpRequestMessage message, int id)
        {
            return FlightStorage.Delete(message, id);
            
        }

    }
}
