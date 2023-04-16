using SadnaExpress.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingBasket
    {
        private Guid storeID;
        public Guid StoreID { get; }
        private Dictionary<int, int> itemsInBasket;
        public Dictionary<int, int> ItemsInBasket { get; }

        public ShoppingBasket(Guid storeId)
        {
            storeID = storeId;
            itemsInBasket = new Dictionary<int, int>();
        }
        
        public override bool Equals(object obj)
        {
            return storeID.Equals(obj);
        }

        public override string ToString()
        {
            string output = $"store ID: {storeID} with the items: \n";
            foreach (int item in itemsInBasket.Keys)
                output += $"item ID: {item} with quantity: {itemsInBasket[item]}\n";
            return output;
        }
        
        /*
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
*/
    }
}