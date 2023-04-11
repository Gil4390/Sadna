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

        public bool AddItem(int itemId, int quantity)
        {
            if (quantity < 0)
                return false;
            
            if (itemsInBasket.ContainsKey(itemId))
            {
                itemsInBasket[itemId] += quantity;
            }
            else
            {
                itemsInBasket.Add(itemId, quantity);
            }

            return true;
        }

    

        // return:
        // 0 - failure 
        // 1 - success
        // 2 basket is not empty so need to remove basket
        public int RemoveItem(int itemId)
        {
            bool result = itemsInBasket.Remove(itemId);
            if(result)
            {
                if (itemsInBasket.Keys.Count.Equals(0))
                    return 2;
                return 1;
            }
            return 0;
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
            return obj is ShoppingBasket && this.storeId.Equals((Guid)obj);
        }


    }
}