using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace FlightPlannerNew.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                var authContext = actionContext.Request.Headers.Authorization.Parameter;
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authContext));
                var userNameAndPassword = decoded.Split(':');
                if (userNameAndPassword[0] == "codelex-admin"
                    && userNameAndPassword[1] == "Password123")
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(userNameAndPassword[0]), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
    }
}