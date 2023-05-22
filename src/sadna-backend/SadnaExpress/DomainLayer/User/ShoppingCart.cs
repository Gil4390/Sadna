using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingCart
    {
        [Key]
        public Guid ShoppingCartId {get; set;}

        public Guid UserId { get; set; }

        private HashSet<ShoppingBasket> baskets;

        public HashSet<ShoppingBasket> Baskets
        {
            get => baskets;
            set => baskets = value;
        }

        public ShoppingCart()
        {
            baskets = new HashSet<ShoppingBasket>();
            ShoppingCartId = Guid.NewGuid();
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
                return 0;
            return shoppingBasket.GetItemQuantity(itemID);
        }

        public void AddUserShoppingCart(ShoppingCart userShoppingCart)
        {
            foreach (ShoppingBasket sb in userShoppingCart.Baskets)
            {
                ShoppingBasket shoppingBasket = null;
                bool exist = baskets.TryGetValue(sb, out shoppingBasket);
                if (exist)
                    shoppingBasket.AddBasket(sb);
                else
                    baskets.Add(sb);
            }
        }
    }
}