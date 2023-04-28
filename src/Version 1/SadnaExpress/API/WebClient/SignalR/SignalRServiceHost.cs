using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace SadnaExpress.API.WebClient.SignalR
{
    public class SignalRServiceHost
    {

        // Create the variable for our self-hosted server

        private IDisposable _server;

        public SignalRServiceHost()
        {

            Console.WriteLine("SignalR Server constructed");
        }

        public void Start()
        {
            Console.WriteLine("SignalR Server started");

            //IApplicationService appService = ServiceLocator.Current.GetInstance<IApplicationService>();
            // appService.SignalRServerUrlPort = appService.GetFreeTcpPort();
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();

            var baseAddress = $"http://localhost:{port}/";

            // Start up the server by providing our OWIN Startup class as the source type.
            //  We also save the return object so we can dispose of it properly when the
            //  service is shutdown
            //
            _server = WebApp.Start<SignalRServerConfig>(url: baseAddress);

            Console.WriteLine($"SignalR Server running at {baseAddress}");
        }

        public void Shutdown()
        {
            Console.WriteLine("SignalR shutting down");

        }

        public void Stop()
        {
            Console.WriteLine("SignalR Server shutting down");

            // Dispose of the server object since we're shutting everything down
            //
            _server.Dispose();

            Console.WriteLine("SignalR stopped");
        }



    }
}
