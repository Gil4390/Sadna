using SadnaExpress.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingBasket
    {
        private Guid storeId;
        //                itemId, quantity Selected
        private Dictionary<int, int> itemsInBasket;

        public ShoppingBasket(Guid store)
        {
            this.storeId = store;
            itemsInBasket = new Dictionary<int, int>();
        }
        
        public void AddItem(int itemId, int quantity)
        {
            if (quantity < 0)
                throw new Exception("cant add item with negative quantity");
            if (itemsInBasket.ContainsKey(itemId))
            {
                itemsInBasket[itemId] += quantity;
            }
            else
            {
                itemsInBasket.Add(itemId, quantity);
            }
        }

        public void RemoveItem(int itemId)
        {
            bool result = itemsInBasket.Remove(itemId);
            if(!result)
            {
                throw new Exception("ItemId not in basket");
            }
        }

        public void EditQuantity(int itemId, int quantity)
        {
            if (quantity < 0)
                throw new Exception("cant edit quantity with negative value");
            else if (quantity == 0)
                RemoveItem(itemId);
            else if (itemsInBasket.ContainsKey(itemId))
                itemsInBasket[itemId] = quantity;
            else 
                throw new Exception("cant edit quantity of item that is not in the basket");
        }

        public Guid GetStoreId()
        {
            return this.storeId;
        }

        public int GetItemStock(int itemId)
        {
            if (itemsInBasket.ContainsKey(itemId))
                return itemsInBasket[itemId];
            throw new Exception("Failure here!");
        }


        public override bool Equals(object obj)
        {
            return obj is ShoppingBasket && this.storeId.Equals(((ShoppingBasket)obj).GetStoreId());
        }

        public Dictionary<int,int> GetItemsInBasket()
        {
            return itemsInBasket;
        }

        public int GetItemsCount()
        { 
            return itemsInBasket.Count; 
        }


    }
}