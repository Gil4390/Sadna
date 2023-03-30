using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ConsoleApp1.DomainLayer;

namespace SadnaExpress
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(delegate ()
            {
                Server myserver = new Server("127.0.0.1", 10011);
            });
            serverThread.Start();
        }
        
    }
    public class Server
    {
        TcpListener server = null;
        UserController userController = new UserController();
        public static Logger logger = new Logger("");

        private bool tradingSystemOpen = false;

        public Dictionary<string, string> Cache = new Dictionary<string, string>();

        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            Serve();
        }

        public void Serve()
        {
            try
            {
                while (!tradingSystemOpen)
                {
                    if (Console.ReadLine() == "Admin Admin")
                        tradingSystemOpen = true;
                    else
                    {
                        Console.WriteLine("Trading system will run when entering valid password");
                    }
                }
                logger.Info("Trading system initialized.");
                while (tradingSystemOpen)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Thread t = new Thread(HandleClient);
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: "+ e);
                server.Stop();
            }
        }
        public void HandleClient(Object obj)
        {

            TcpClient client = (TcpClient)obj;
            Guest g = new Guest(client);
            userController.addUser(g);
            logger.Info(g , "guest entered the system");
            var stream = client.GetStream();


            string messageFromClient = "";
            string responseToClient = "";
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while (true)
                {
                    Byte[] reply;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        //transle from bytes to string the message send from the client
                        messageFromClient = Encoding.ASCII.GetString(bytes, 0, i);

                        //create proper response to the client
                        responseToClient = "Response";
                        reply = System.Text.Encoding.ASCII.GetBytes(responseToClient);
                        stream.Write(reply, 0, reply.Length);
                    }
                    responseToClient = "Check-For-Connection";
                    reply = System.Text.Encoding.ASCII.GetBytes(responseToClient);
                    stream.Write(reply, 0, reply.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection lost");
                userController.removeUser(g);
                client.Close();
            }
        }

    }
}