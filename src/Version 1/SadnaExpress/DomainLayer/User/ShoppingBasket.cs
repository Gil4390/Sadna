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

        public int getAvailableItemQuantity(int itemId)
        {
            return TradingSystem.Instance.GetAvailableQuantity(this.storeId, itemId);
        }

        public bool AddItem(int itemId, int quantity)
        {
            if (quantity < 0)
                return false;

            int storeItemAvailableQuantity = getAvailableItemQuantity(itemId);

            if (storeItemAvailableQuantity > quantity)
            {
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
            else
            {
                throw new Exception("cant add item to shopping basket with stock more that the store can provide!");
            }

            throw new Exception("tried to add item to shopping basket that is not in the store of this shopping basket");
            return false;
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
            {
                int storeItemAvailableQuantity = getAvailableItemQuantity(itemId);
                if(storeItemAvailableQuantity > quantity)
                {
                    itemsInBasket[itemId] = quantity;
                }
                else
                {
                    throw new Exception("cant edit quantity in basket because store doesnt have the availble quantity of this item");
                }
            }
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