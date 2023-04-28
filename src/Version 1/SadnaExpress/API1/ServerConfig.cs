using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


[assembly: OwinStartup(typeof(SadnaExpress.API1.ServerConfig))]

namespace SadnaExpress.API1
{
    public class ServerConfig
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure CORS to allow JavaScript clients from any
            // domain to access our REST API
            object value = appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            HttpConfiguration config = new HttpConfiguration();

            // Configure REST API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configure JSON Formatter for REST API
            var jsonFormatter = new JsonMediaTypeFormatter();

            jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Formatters.Clear();
            config.Formatters.Insert(0, jsonFormatter);

            // Add global action filters -> Should refactor all controllers to not have their own logic of errors
            //config.Filters.Add(new ErrorFilterAttribute());

            // Configure REST API
            appBuilder.UseWebApi(config);

            config.EnsureInitialized();

        }
    }
}
