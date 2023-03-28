using System.Net.Sockets;

class ClientProgram
{

    /// <summary>
    /// Main client function
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        RunClient("127.0.0.1");
        Console.ReadLine();
    }

    /// <summary>
    /// The function connact to the server and runs the client
    /// </summary>
    /// <param name="server">the server to connact to</param>
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
                if (IsValidMessage(messageToServer))
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

    /// <summary>
    /// The function checks if the string enterd by the client , is valid message to the server.
    /// There are 2 types of valid massages:
    /// =>get command: get X
    /// =>set command: set X int\r\n Y
    /// </summary>
    /// <param name="mes">the message , enterd by the client</param>
    /// <returns></returns>
    public static bool IsValidMessage(string mes)
    {
        string[] splitMessage = mes.Split(" ");
        if (splitMessage[0].Equals("get"))
        {
            //Is get message
            return splitMessage.Length < 3;
        }
        if (splitMessage[0].Equals("set"))
        {
            //Is set message
            int len = 0;
            for (int i = 3; i < splitMessage.Length; i++)
            {
                len += splitMessage[i].Length;
            }

            len += splitMessage.Length - 4;
            return splitMessage.Length > 3 && len == int.Parse(splitMessage[2].Split('\\')[0]);
        }
        return false;
    }
}