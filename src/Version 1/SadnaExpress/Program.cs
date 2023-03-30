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
            Console.WriteLine("Trading system started");
            
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

        public Dictionary<string, string> Cache = new Dictionary<string, string>();

        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            logger.Info("guest entered the system");
            server.Start();
            Serve();
        }

        public void Serve()
        {
            try
            {
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Guest g = new Guest(client);
                    userController.addUser(g);
                    logger.Info(g , "guest entered the system");
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
            var stream = client.GetStream();

            string messageFromClient = "";
            string responseToClient = "";
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //transle from bytes to string the message send from the client
                    messageFromClient = Encoding.ASCII.GetString(bytes, 0, i);

                    //create proper response to the client
                    responseToClient = "Response";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(responseToClient);
                    stream.Write(reply, 0, reply.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: "+ e.ToString());
                client.Close();
            }
        }

    }
}