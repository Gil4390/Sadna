using SadnaExpress.DomainLayer.Store;
using System.Net.Sockets;

namespace SadnaExpress.DomainLayer.User
{

    public class Guest : User
    {

        private ShoppingCart cart;


        public Guest(int id): base(id)
        {
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