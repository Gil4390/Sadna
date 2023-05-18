using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Owin.Hosting;
using SadnaExpress.API;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using SadnaExpress.API.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace SadnaExpress
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello Sadna");
            TradingSystem.Instance.SetIsSystemInitialize(true);
            TradingSystem.Instance.LoadData();
           // TradingSystem.Instance.SetIsSystemInitialize(false);

            //start the api server
            ServerServiceHost serverServiceHost = new ServerServiceHost();
            serverServiceHost.Start();
            
            //start the signalR server
            SignalRServiceHost signalRServiceHost = new SignalRServiceHost();
            signalRServiceHost.Start();

            
            try
            {            
                string address = "https://php-server-try.000webhostapp.com/";

                // Create a new HttpClient instance
                HttpClient client = new HttpClient();

                var postContent = new Dictionary<string, string>
                {
                    {"action_type","handshake"},
                };

                // Convert the dictionary to form URL-encoded content
                var formData = new FormUrlEncodedContent(postContent);

                // Send the HTTP POST request
                var responseTask = client.PostAsync(address, formData);

                // Wait for the response
                var response = responseTask.Result;

                // Read the response content as a string
                var responseContentTask = response.Content.ReadAsStringAsync();
                var responseContent = responseContentTask.Result;

                // Display the response
                Console.WriteLine(responseContent);

                // Dispose the HttpClient to release resources
                client.Dispose();

            }
            catch (WebException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.ReadLine();
        }



    }
}