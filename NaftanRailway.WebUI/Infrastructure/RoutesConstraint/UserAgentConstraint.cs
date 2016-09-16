using System.Web;
using System.Web.Routing;

namespace NaftanRailway.WebUI.Infrastructure.RoutesConstraint {
   /* The IRouteConstraint interface defines the Match method, which an implementation can use to indicate to 
        the routing system if its constraint has been satisfied. The parameters for the Match method provide access to the 
        request from the client, the route that is being evaluated, the parameter name of the constraint, the segment variables 
        extracted from the URL, and details of whether the request is to check an incoming or outgoing URL. 
    */
    public class UserAgentConstraint : IRouteConstraint {
        private readonly string _requiredUserAgent;
 
        public UserAgentConstraint(string agentParam) {
            _requiredUserAgent = agentParam;
        }
    //Match if request from specific browser-agent
        public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                          RouteValueDictionary values, RouteDirection routeDirection) {
 
            return httpContext.Request.UserAgent != null &&
                httpContext.Request.UserAgent.Contains(_requiredUserAgent);
        }
    }
}