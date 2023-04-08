using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingBasket
    {
        private string store;
        //                 itemId, stockSelected
        private Dictionary<int,int> itemsInBasket;

        public ShoppingBasket(string store)
        {
            this.store = store;
            this.itemsInBasket = new Dictionary<int, int>();
        }

        public bool AddItem(int itemId, int stock)
        {
            if (stock < 0)
                return false;

            List<Inventory> storeInventory = ((Store)DomainFacade.Instance.GetStore(store)).getItemsInventory();

            foreach (Inventory item in storeInventory)
            {
                if (item.GetId() == itemId)
                {
                    if (item.getInStock() > stock)
                    {
                        if (itemsInBasket.ContainsKey(itemId))
                        {
                            itemsInBasket[itemId] += stock;
                        }
                        else
                        {
                            itemsInBasket.Add(itemId, stock);
                        }
                        return true;
                    }
                    else
                    {
                        throw new Exception("cant add item to shopping basket with stock more that the store can provide!");
                    }
                }
            }
            throw new Exception("tried to add item to shopping basket that is not in the store of this shopping basket");
            return false;
        }


        public bool RemoveItem(int itemId)
        {
            return itemsInBasket.Remove(itemId);
        }

        public bool EditAmount(int itemId, int newStock)
        {
            if (newStock < 0)
                return false;
            if (newStock == 0)
                return RemoveItem(itemId);
            if (itemsInBasket.ContainsKey(itemId))
            {
                int abs = itemsInBasket[itemId] - newStock;
                if (abs < 0)
                    abs = abs * -1;
                return AddItem(itemId, abs);
            }
            return false;
        }

        public string GetStore()
        {
            return this.store;
        }

        public int GetItemStock(int itemId)
        {
            if (itemsInBasket.ContainsKey(itemId))
                return itemsInBasket[itemId];
            throw new Exception("Failure here!");
        }


    }
}