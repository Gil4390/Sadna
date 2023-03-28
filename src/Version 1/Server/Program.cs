using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ServerProgram
{

    /// <summary>
    /// Main Server function, initailize the server thread and runs it.
    /// </summary>
    /// <param name="args"></param>
    // public static void Main(string[] args)
    // {
    // Thread t = new Thread(delegate ()
    // {
    //     // replace the IP with your system IP Address...
    //     Server myserver = new Server("127.0.0.1", 10011);
    // });
    // t.Start();
    //     
    // Console.WriteLine("Server Started...!");
    // }

    public class Server
    {
        TcpListener server = null;

        public Dictionary<string, string> Cache = new Dictionary<string, string>();
        private int CacheSize = 0;
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
                while (true)
                {
                    if (Cache.Count == 0) 
                        Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    //Console.WriteLine("new Connection!");
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
        /// <summary>
        /// The function handle a connection of client :
        /// while client connected , gets command to be excuted on the cache and returns proper response
        /// </summary>
        /// <param name="obj">The new TCO client</param>
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
                    responseToClient = ExcuteMessage(messageFromClient);
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
        /// <summary>
        /// The function gets command from client , exctue it on the cache and create proper resonse
        /// IF recived get x command:
        /// => returns OK\r\n if key exist in cahce
        /// => returns MISSING\r\n if key dosn't exist in cahce
        /// 
        /// IF recived set x int\r\n string command:
        /// => set the value and returns OK\r\n
        /// 
        /// </summary>
        /// <param name="mes">the command to be exucte</param>
        /// <returns>the response to the client</returns>
        public string ExcuteMessage(string mes)
        {

            string result = "";
            string[] splitMessage = mes.Split(" ");
            
            //Is get message recived
            if (splitMessage[0].Equals("get"))
            {
                if (Cache.ContainsKey(mes.Split("get ")[1]))
                    result = "OK "+ Cache[splitMessage[1]].Length+"\r\n"+Cache[splitMessage[1]];
                else
                    result = "MISSING\r\n";
            }
            
            //Is set message recived
            if (splitMessage[0].Equals("set"))
            {
                CacheManagement(int.Parse(splitMessage[2].Split('\\')[0]));
                if (splitMessage.Length > 4)
                    for (int i = 4; i < splitMessage.Length; i++)
                        splitMessage[3] +=" "+splitMessage[i];
                Cache[splitMessage[1]] = splitMessage[3];
                result = "OK\r\n";
            }
            return result;
        }

        /// <summary>
        /// The function manages the memory size in the cache , when command set excute
        /// IF the new value does exceed the maximum size, the function chooses a valuerandomly and delete it from the cache.
        /// IF the new value still exceed the maximum size , the function will repeat the operation until there is enough space for the new valueץ
        /// </summary>
        /// <param name="newValueSize"></param>
        public void CacheManagement(int newValueSize)
        {
            //128 megabytes = 128 000 000 bytes
            while (CacheSize + newValueSize > 128000000)
            {
                Random random = new Random();
                string valueToRemove = Cache.ElementAt(random.Next(Cache.Count)).Key;
                Cache.Remove(valueToRemove);
            }
            CacheSize += newValueSize;
        }
    }
}