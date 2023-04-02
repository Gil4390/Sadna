using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;

namespace SadnaExpress
{
    public class Program
    {
        public static Logger logger = new Logger("");
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(delegate ()
            {
                Server myserver = new Server("127.0.0.1", 10011);
            });
            serverThread.Start();
        }    
    }
}