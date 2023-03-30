using System.Net;

namespace ConsoleApp1.DomainLayer;

public class User
{
    protected int userId;
    protected ShoppingCart shoppingCart;

    public User()
    {
        shoppingCart = new ShoppingCart();
        userId = 0;
    }

    public int convertToInt(EndPoint ep)
    {
        string newEP = ep.ToString().Split(":")[1];
        int parseId = int.Parse(newEP);
        return parseId;
    }
}