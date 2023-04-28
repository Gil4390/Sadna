using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;


[assembly: OwinStartup(typeof(SadnaExpress.API1.WebClient.WebClientServer.WebClientServerConfig))]

namespace SadnaExpress.API1.WebClient.WebClientServer
{
    public class WebClientServerConfig
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure CORS to allow JavaScript clients from any
            // domain to access our REST API
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            string clientAppPath = "build";
            if (Directory.Exists(clientAppPath) == false)
            {
                Directory.CreateDirectory(clientAppPath);
            }

            FileServerOptions options = new FileServerOptions()
            {
                EnableDirectoryBrowsing = true,
                FileSystem = new PhysicalFileSystem($"./{clientAppPath}"),
            };

            options.StaticFileOptions.ServeUnknownFileTypes = true;

            appBuilder.UseFileServer(options);

            HttpConfiguration config = new HttpConfiguration();

            // Configure REST API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configure REST API
            appBuilder.UseWebApi(config);

            config.EnsureInitialized();

        }
    }
}
