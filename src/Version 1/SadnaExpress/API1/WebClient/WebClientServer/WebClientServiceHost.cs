using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.API1.WebClient.WebClientServer
{
    public class WebClientServiceHost
    {
        // Create the variable for our self-hosted server
        //
        private IDisposable _server;


        public WebClientServiceHost()
        {
            Console.WriteLine("WebClient Server constructed");
        }

        public void Start()
        {
            Console.WriteLine("WebClient Server started");

            // IApplicationService appService = ServiceLocator.Current.GetInstance<IApplicationService>();
            //appService.WebBrowserServerUrlPort = appService.GetFreeTcpPort();
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            var baseAddress = $"http://localhost:{port}/";

            // Start up the server by providing our OWIN Startup class as the source type.
            //  We also save the return object so we can dispose of it properly when the
            //  service is shutdown
            //
            _server = WebApp.Start<WebClientServerConfig>(url: baseAddress);

            Console.WriteLine($"WebClient Server running at {baseAddress}");
        }

        public void Shutdown()
        {
            Console.WriteLine("WebClientServiceHost shutting down");

        }

        public void Stop()
        {
            Console.WriteLine("WebClient Server shutting down");

            // Dispose of the server object since we're shutting everything down
            //
            _server.Dispose();

            Console.WriteLine("WebClientServiceHost stopped");
        }
    }
}
