using System.Net;

using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User { 
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
        string newEP = ep.ToString().Split(':')[1];
        int parseId = int.Parse(newEP);
        return parseId;
    }
    public int UserId
    {
        get => userId;
        set => userId = value;
    }
}
    }