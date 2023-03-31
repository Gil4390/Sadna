using ConsoleApp1.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingCart
    {
        private List<ShoppingBasket> baskets;
        // list for saved items

        public ShoppingCart()
        {
            baskets = new List<ShoppingBasket>();
        }
        public ShoppingCart(List<ShoppingBasket> baskets)
        {
            baskets = baskets;
        }


        // functions to implement
        
        // getters


        // add / remove basket
        
        // add / remove items

        //  purchase basket

        // purchase items

        // get num of items

        // edit stock of item

        // save items for purchase




    }
}