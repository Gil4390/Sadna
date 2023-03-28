namespace ConsoleApp1;

public class TradingSystem
{
    public static void Main(string[] args)
    {
        Thread t = new Thread(delegate ()
        {
            // replace the IP with your system IP Address...
            ServerProgram.Server myserver = new ServerProgram.Server("127.0.0.1", 10011);
        });
        t.Start();
        
        Console.WriteLine("Server Started...!");
    }
}