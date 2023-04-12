using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingCart
    {
        private HashSet<ShoppingBasket> baskets;
        // list for saved items

        public ShoppingCart()
        {
            baskets = new HashSet<ShoppingBasket>();
        }
        public HashSet<ShoppingBasket> GetShoppingBaskets()
        {
            return this.baskets;
        }


        public ShoppingBasket GetShoppingBasketByStore(Guid store)
        {
            foreach (ShoppingBasket basket in baskets)
            {       
                if (basket.GetStoreId().Equals(store))
                {
                    return basket;
                }
            }
            return null;
        }

        public void AddBasket(ShoppingBasket basket)
        {
            foreach (ShoppingBasket b in baskets)
            {
                if (b.Equals(basket))
                    throw new Exception("Failed to add basket (this basket already exists in the cart)");
            }
            baskets.Add(basket);
        }

        public void RemoveBasket(ShoppingBasket basket)
        {
            baskets.Remove(basket);
        }

        public void AddItemToBasket(Guid storeId, int itemId, int stock)
        {
            bool addShoppingBasket = true;
            foreach (ShoppingBasket b in baskets)
            {
                if (b.GetStoreId().Equals(storeId))
                {
                    b.AddItem(itemId, stock);
                    addShoppingBasket = false;
                }
            }
            if (addShoppingBasket)
            {
                ShoppingBasket newOne = new ShoppingBasket(storeId);
                newOne.AddItem(itemId, stock);
                baskets.Add(newOne);
            }
        }


        public void RemoveItemFromBasket(Guid storeId, int itemId)
        {
            foreach (ShoppingBasket b in baskets)
            {
                if (b.GetStoreId().Equals(storeId))
                {
                    b.RemoveItem(itemId);
                    int amount = b.GetItemsCount();
                    if (amount.Equals(0))
                        this.RemoveBasket(b);
                }
            }
        }




    }
}