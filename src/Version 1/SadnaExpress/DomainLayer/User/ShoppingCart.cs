using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingCart
    {
        private List<ShoppingBasket> baskets;
        // list for saved items

        public ShoppingCart()
        {
            baskets = new List<ShoppingBasket>();
        }


        public List<ShoppingBasket> GetShoppingBaskets()
        {
            return this.baskets;
        }

        public ShoppingBasket GetShoppingBasketByStore(string store)
        {
            foreach (ShoppingBasket basket in baskets)
            {
                if (basket.GetStore().Equals(store))
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
                if (b.GetStore() == basket.GetStore())
                    return false;
            }
            baskets.Add(basket);
            return true;
        }

        public bool RemoveBasket(ShoppingBasket basket)
        {
            return baskets.Remove(basket);
        }

        public bool AddItemToBasket(string storeName, int itemId, int stock)
        {
            foreach (ShoppingBasket b in baskets)
            {
                if (b.GetStore() == storeName)
                {
                    return b.AddItem(itemId, stock);
                }
            }
            ShoppingBasket newOne = new ShoppingBasket(storeName);
            newOne.AddItem(itemId, stock);
            baskets.Add(newOne);
            return true;
        }


        public bool RemoveItemFromBasket(string storeName, int itemId)
        {
            foreach (ShoppingBasket b in baskets)
            {
                if (b.GetStore() == storeName)
                {
                    return b.RemoveItem(itemId);
                }
            }
            return false;
        }




    }
}