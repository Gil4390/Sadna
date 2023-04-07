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
        // public static void Main(string[] args)
        // {
        //     Console.WriteLine("Server Started!");
        //     Console.WriteLine("In order to login enter: Admin Admin");
        //     Thread serverThread = new Thread(delegate()
        //     {
        //         Server myserver = new Server("127.0.0.1", 10011,null, null);
        //     });
        //     serverThread.Start();
        // }
        public static void Main(string[] args)
        {
            Server sc = new Server();
            Queue<string> commands = new Queue<string>();
            commands.Enqueue("REGISTER tal.galmor@weka.io tal galmor password");
            commands.Enqueue("LOGIN tal.galmor@weka.io password");
            //commands.Enqueue("LOGIN tal.galmor@weka.io password");
            Thread workerOne = new Thread(() => sc.ServeClient("Elly",commands));
            workerOne.Start();
        }
    }
}