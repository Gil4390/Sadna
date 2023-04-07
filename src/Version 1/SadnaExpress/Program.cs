using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;

namespace SadnaExpress
{
    internal class Program
    {
        private static Server _server;
        public static void Main(string[] args)
        {
            new IntegrationTests().Run();
        }

        public class IntegrationTests
        {
            public void Run()
            {
                Check_Init_system();
            }
            public void SetUp()
            {
                _server = new Server();
            }

            public void StartSystem()
            {
                _server.activateAdmin();
            }

            public void RunClient(string name , Queue<string> commands)
            {
                Thread client1 = new Thread(() => _server.ServeClient(name,commands));
                client1.Start();
            }
            /// <summary>
            /// Use case - check if client can register to the system without admin init it.
            /// </summary>
            public void Check_Init_system()
            {
                SetUp();
                Queue<string> commands = new Queue<string>();
                commands.Enqueue("REGISTER tal.galmor@weka.io tal galmor password");
                RunClient("Elly", commands);
                if (_server.tradingSystemOpen || _server.service.GetCurrent_Users().ContainsKey(1))
                    Console.WriteLine("WRONG");
            }
        }
        
    }
}