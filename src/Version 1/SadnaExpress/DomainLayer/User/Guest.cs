using System.Net.Sockets;

namespace ConsoleApp1.DomainLayer;

public class Guest : User
{
    public Guest(TcpClient client)
    {
        userId = convertToInt(client.Client.RemoteEndPoint);
    }
}