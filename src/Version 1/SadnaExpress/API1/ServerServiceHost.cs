using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.API1
{
    public class ServerServiceHost
    {
        // Create the variable for our self-hosted server

        private IDisposable _server;


        public ServerServiceHost()
        {
            Console.WriteLine("Data Server constructed");
        }

        public void Start()
        {
            try
            {
                Console.WriteLine("Date Server started");
                Logger.Instance.Info($"{nameof(ServerServiceHost)} - Date Server started.");

                int port = 8080;

                var baseAddress = $"http://localhost:{port}/";

                // Start up the server by providing our OWIN Startup class as the source type.
                //  We also save the return object so we can dispose of it properly when the
                //  service is shutdown
                //
                _server = WebApp.Start<ServerConfig>(url: baseAddress);

                Logger.Instance.Info($"{nameof(ServerServiceHost)} - Server running at {baseAddress}.");
                Console.WriteLine($"Server running at {baseAddress}");

            }
            catch (Exception ex)
            {
                Logger.Instance.Info($"{nameof(ServerServiceHost)} - Failed to start server, Exception: [{ex}].");
            }
        }

        public void Shutdown()
        {
            Console.WriteLine("ServiceHost shutting down");

        }

        public void Stop()
        {
            Console.WriteLine("Server shutting down");

            // Dispose of the server object since we're shutting everything down
            //
            _server.Dispose();

            Console.WriteLine("ServiceHost stopped");
        }
    }
}
