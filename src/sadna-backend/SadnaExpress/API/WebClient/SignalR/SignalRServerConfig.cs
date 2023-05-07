//using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using SadnaExpress.API.WebClient.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(SadnaExpress.API.WebClient.SignalR.SignalRServerConfig))]

namespace SadnaExpress.API.WebClient.SignalR
{
    public class SignalRServerConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                var config = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting this line
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    //EnableJSONP = true
                    EnableDetailedErrors = true
                };

                // Turns cors support on allowing everything
                // In real applications, the origins should be locked down
                map.UseCors(CorsOptions.AllowAll)
                   .RunSignalR(config);

            });

        }
    }
}
