using SadnaExpress.DomainLayer.User;
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
            Console.WriteLine("Client connected!");
            int id = service.enter(); // each handler gets a new id, if user loggs in the id will change
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
                        Console.WriteLine(messageFromClient);

                        //responseToClient = parse(messageFromClient);



                        string[] split = messageFromClient.Split(' ');
                        string command_type = split[0];

                        if (command_type == "EXIT")
                        {
                            //EXIT
                            service.exit(id); 
                            client.Close();
                            Console.WriteLine("Client exited");
                        }
                        else if (command_type == "REGISTER")
                        {
                            //REGISTER <email> <firstName> <lastName> <password>
                            if (split.Length != 5) { throw new Exception("invalid register args"); }
                            string email = split[1];
                            string firstName = split[2];
                            string lastName = split[3];
                            string password = split[4];
                            service.register(id, email, firstName, lastName, password);

                        }
                        else if (command_type == "LOGIN")
                        {
                            //LOGIN <email> <password>
                            if (split.Length != 3) { throw new Exception("invalid login args"); }
                            string email = split[1];
                            string password = split[2];
                            id = service.login(id, email, password);
                        }
                        else if (command_type == "LOGOUT")
                        {
                            //LOGOUT
                            id = service.logout(id);
                        }



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
                service.exit(id);
                client.Close();
            }
        }

        public string parse(string msg)
        {
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