using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlightPlannerNew.Models;

namespace FlightPlannerNew.Controllers
{
    public class TestingController : ApiController
    {
        [HttpPost, Route("testing-api/clear")]
        public HttpResponseMessage Post(HttpRequestMessage message)
        {
            FlightStorage.ClearFLights();
            return message.CreateResponse(HttpStatusCode.OK);
        }

    }
}