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
                var flight = FlightStorage.FlightsDataBase.FirstOrDefault(x => x.Id == id);
                if (flight == null)
                {
                    return message.CreateResponse(HttpStatusCode.NotFound);
                }
                return message.CreateResponse(HttpStatusCode.OK, flight);
            }

        // GET: api/Admin/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPut, Route("admin-api/flights")]
        // PUT: api/Admin/5
        public HttpResponseMessage Put(HttpRequestMessage message, Flight flight)
        {
            foreach (var trip in FlightStorage.FlightsDataBase)
            {
                if (trip.Id == flight.Id)
                {
                    flight.Id = FlightStorage.GetRandomFlightId();
                }
            }

            FlightStorage.AddFlights(flight);

            return message.CreateResponse(HttpStatusCode.Created, flight);

        }

        // DELETE: api/Admin/5
        [HttpDelete, Route("admin-api/flights/{id}")]
        public HttpResponseMessage Delete(HttpRequestMessage message, int id)
        {

        }
    }
}
