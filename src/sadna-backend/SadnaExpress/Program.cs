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
            Console.ReadLine();
        }



    }
}