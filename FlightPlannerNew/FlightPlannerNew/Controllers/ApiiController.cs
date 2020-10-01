using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightPlannerNew.Models;

namespace FlightPlannerNew.Controllers
{
    public class ApiiController : ApiController
    {
        // GET: api/Apii
        [HttpGet, Route("api/airports")]
        public HttpResponseMessage Get(HttpRequestMessage message, string search)
        {
            return FlightStorage.GetTheAirport(message, search);
        }

        // POST: api/Apii
        [HttpPost, Route("api/flights/search")]
        public HttpResponseMessage Post(HttpRequestMessage message, SearchFlightRequest searchFlight)
        {
            if (!ModelState.IsValid)
            {
                return message.CreateResponse(HttpStatusCode.BadRequest);
            }


            return FlightStorage.SearchForAirport(message, searchFlight);

        }

        // PUT: api/Apii/5
        [HttpGet, Route("api/flights/{id}")]
        public HttpResponseMessage Get(HttpRequestMessage message, int id)
        {
            return FlightStorage.SearchFlightById(message, id);
        }

        // DELETE: api/Apii/5
        public void Delete(int id)
        {
        }
    }
}