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

        public bool AddBasket(ShoppingBasket basket)
        {
            foreach (ShoppingBasket b in baskets)
            {
                if (b.Equals(basket))
                    return false;
            }
            baskets.Add(basket);
            return true;
        }

        public bool RemoveBasket(ShoppingBasket basket)
        {
            return baskets.Remove(basket);
        }

        public bool AddItemToBasket(Guid storeId, int itemId, int stock)
        {
            foreach (ShoppingBasket b in baskets)
            {
                if (b.GetStoreId().Equals(storeId))
                {
                    return b.AddItem(itemId, stock);
                }
            }
            ShoppingBasket newOne = new ShoppingBasket(storeId);
            newOne.AddItem(itemId, stock);
            baskets.Add(newOne);
            return true;
        }


        public bool RemoveItemFromBasket(Guid storeId, int itemId)
        {
            foreach (ShoppingBasket b in baskets)
            {
                if (b.GetStoreId().Equals(storeId))
                {
                    int result = b.RemoveItem(itemId);
                    if (result == 0)
                        return false;
                    if (result == 2)
                        this.RemoveBasket(b);
                    return true;
                }
            }
            return false;
        }




    }
}