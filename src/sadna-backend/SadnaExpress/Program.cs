using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Owin.Hosting;
using SadnaExpress.API;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using SadnaExpress.API.WebClient.SignalR;
using SadnaExpress.API.WebClient.WebClientServer;

namespace SadnaExpress
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello Sadna");
            TradingSystem.Instance.SetIsSystemInitialize(true);

            //start the api server
            ServerServiceHost serverServiceHost = new ServerServiceHost();
            serverServiceHost.Start();
            Console.ReadLine();

            //start the web server
            // WebClientServiceHost webClientServiceHost = new WebClientServiceHost();
            // webClientServiceHost.Start();

            //start the signalR server
            // SignalRServiceHost signalRServiceHost = new SignalRServiceHost();
            // signalRServiceHost.Start();

        }



    }
}