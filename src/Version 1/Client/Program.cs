using System;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            RunClient("127.0.0.1");
            Console.ReadLine();
        }
        
        static void RunClient(String server) 
        {
            try 
            {
                Int32 port = 10011;
                TcpClient client = new TcpClient(server, port);

                NetworkStream stream = client.GetStream();
                string messageToServer = "";
                string responseFromServer = "";
                Byte[] data;
                int count = 0;
                while (!messageToServer.Equals("disconnect"))
                {
                    messageToServer = Console.ReadLine();
                    if (messageToServer != "")
                    {
                        // Translate the Message into ASCII.
                        data = System.Text.Encoding.ASCII.GetBytes(messageToServer);   

                        // Send the message to the connected TcpServer. 
                        stream.Write(data, 0, data.Length);
                        // Bytes Array to receive Server Response.
                        data = new Byte[256];

                        // Read the Tcp Server Response Bytes.
                        Int32 bytes = stream.Read(data, 0, data.Length);
                        responseFromServer = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        Console.WriteLine(responseFromServer);
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        Console.WriteLine("Message isn't valid");
                    }

                }
                stream.Close();         
                client.Close();         
            } 
            catch (Exception e) 
            {
                Console.WriteLine("Exception: "+ e);
            }

            Console.Read();
        }
    }
}