using System.Collections.Generic;

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

        internal void addItem(Inventory inv, int stock)
        {
            itemsInBasket.Add(inv, stock);
        }

        internal Store getStore()
        {
            return this.store;
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