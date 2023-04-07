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
                Test1();
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

            public void Test1()
            {
                SetUp();
                StartSystem();
                Queue<string> commands = new Queue<string>();
                commands.Enqueue("REGISTER tal.galmor@weka.io tal galmor password");
                commands.Enqueue("LOGIN tal.galmor@weka.io password");
                RunClient("Elly", commands);
            }
        }
        
    }
}