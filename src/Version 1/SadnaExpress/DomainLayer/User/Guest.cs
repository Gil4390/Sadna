using SadnaExpress.DomainLayer.Store;
using System.Net.Sockets;

namespace SadnaExpress.DomainLayer.User
{

    public class Guest : User
    {

        private ShoppingCart cart;


        public Guest(TcpClient client)
        {
            userId = convertToInt(client.Client.RemoteEndPoint);

            this.cart = new ShoppingCart(new List<ShoppingBasket>());
            
        }


        public ShoppingCart GetShoppingCart()
        {
            return this.cart;
        }

        public bool addInventoryToCart(Inventory inv, int stock)
        {
            return this.cart.addInventoryToCart(inv, stock);
        }



    }
}