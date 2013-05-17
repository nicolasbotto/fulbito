using MvcWebRole.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace MvcWebRole
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Self referencing support
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // Add Jsonp support
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonpFormatter(jsonSerializerSettings));

            // Add Cors support
            GlobalConfiguration.Configuration.MessageHandlers.Add(new CorsHandler()); 
           // CorsConfig.RegisterCors(GlobalConfiguration.Configuration);

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            
        }
    }

    public class CorsConfig
{
    public static void RegisterCors(HttpConfiguration httpConfig)
    {
        WebApiCorsConfiguration corsConfig = new WebApiCorsConfiguration();
 
        // this adds the CorsMessageHandler to the HttpConfiguration’s
        // MessageHandlers collection
        corsConfig.RegisterGlobal(httpConfig);
       
        // this allow all CORS requests to the Products controller
        // from the http://foo.com origin.
        corsConfig
            .ForResources("Appointments")
            .ForOrigins("*")
            .AllowAll();
    }
}
}