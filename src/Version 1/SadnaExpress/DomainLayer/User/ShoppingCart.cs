using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingCart
    {
        private HashSet<ShoppingBasket> baskets;

        public HashSet<ShoppingBasket> Baskets
        {
            get => baskets;
        }

        public ShoppingCart()
        {
            baskets = new HashSet<ShoppingBasket>();
        }

        public override string ToString()
        {
            string output = "";
            int i = 1;
            foreach (ShoppingBasket basket in baskets)
            {
                output += $"{i}. {basket} \n";
                i++;
            }

            return output;
        }

        public void AddItemToCart(Guid storeID, Guid itemID, int stock)
        {
            ShoppingBasket shoppingBasket = new ShoppingBasket(storeID);
            baskets.Add(shoppingBasket);
            baskets.TryGetValue(shoppingBasket, out shoppingBasket);
            shoppingBasket.AddItem(itemID, stock);
        }

        public void RemoveItemFromCart(Guid storeID, Guid itemID)
        {
            ShoppingBasket shoppingBasket = new ShoppingBasket(storeID);
            bool exist = baskets.TryGetValue(shoppingBasket, out shoppingBasket);
            if (!exist)
                throw new Exception("The cart doesn't include the store");
            shoppingBasket.RemoveItem(itemID);
            if (shoppingBasket.ItemsInBasket.Count == 0)
                baskets.Remove(shoppingBasket);
        }
        
        public void EditItemFromCart(Guid storeID, Guid itemID, int itemAmount)
        {
            ShoppingBasket shoppingBasket = new ShoppingBasket(storeID);
            bool exist = baskets.TryGetValue(shoppingBasket, out shoppingBasket);
            if (!exist)
                throw new Exception("The cart doesn't include the store");
            shoppingBasket.EditItem(itemID, itemAmount);
            if (shoppingBasket.ItemsInBasket.Count == 0)
                baskets.Remove(shoppingBasket);
        }

        public int GetItemQuantityInCart(Guid storeID, Guid itemID)
        {
            ShoppingBasket shoppingBasket = new ShoppingBasket(storeID);
            bool exist = baskets.TryGetValue(shoppingBasket, out shoppingBasket);
            if (!exist)
                throw new Exception("The cart doesn't include the store");
            return shoppingBasket.GetItemQuantity(itemID);  
        }
    }
}