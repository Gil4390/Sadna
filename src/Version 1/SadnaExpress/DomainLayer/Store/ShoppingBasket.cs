using ConsoleApp1.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingBasket
    {
        private Store store;
        private Dictionary<Inventory,int> itemsInBasket;

        public ShoppingBasket(Store store, Dictionary<Inventory, int> itemsInBasket)
        {
            this.store = store;
            this.itemsInBasket = itemsInBasket;
        }

        // functions to implement:

        // getters

        // setters

        // add Item

        // delete Item

        // edit stock in basket

        // get items numbers in basket

        // find item in basket


    }
}