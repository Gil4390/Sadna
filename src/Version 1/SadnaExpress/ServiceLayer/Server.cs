using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SadnaExpress.ServiceLayer
{
    public class Server
    {
        TcpListener server = null;

        private bool tradingSystemOpen = false;

        public Tradingsystem service;

        public Dictionary<string, string> Cache = new Dictionary<string, string>();

        public Server(string ip, int port)
        {
            service = new Tradingsystem();
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
                while (tradingSystemOpen)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Thread t = new Thread(HandleClient);
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: " + e);
                server.Stop();
            }
        }
        public void HandleClient(Object obj)
        {

            TcpClient client = (TcpClient)obj;
            service.enter(int.Parse(client.Client.RemoteEndPoint.ToString().Split(':')[1]));

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
                        messageFromClient = Encoding.ASCII.GetString(bytes, 0, i);

                        responseToClient = parse(messageFromClient);
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
                service.exit(int.Parse(client.Client.RemoteEndPoint.ToString().Split(':')[1]));
                client.Close();
            }
        }

        public string parse(string msg)
        {
            string res = "";
            //Register (username) (password)
            //Login (username) (password)
            //Logout
            //Exit system
            //Open store (name) ...
            //Close store (name) ...
            //Purchase
            //Show history (storeName)
            //Add (itemName) (StoreName)
            //Remove (itemName) (StoreName)
            //Edit (itemName) (category)
            //Edit (itemName) (price)
            //Get (temName)
            //Get (category)
            //Get (keyWords)
            //Get (minPrice) (maxPrice)
            //Get (storeName) (rating)
            //Get (itemName) (rating)
            //Review (itemName) (Store)


            return "";
        }

    }
}