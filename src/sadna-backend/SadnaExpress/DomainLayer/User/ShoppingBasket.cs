using Newtonsoft.Json;
using SadnaExpress.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SadnaExpress.DomainLayer.Store
{
    public class ShoppingBasket
    {
        [Key]
        public Guid ShoppingBasketId { get; set; }

        public Guid ShoppingCartId { get; set; }
        //public ShoppingCart ShoppingCart { get; set; } // added for map 

        private Guid storeID;
        public Guid StoreID { get => storeID; set => storeID = value; }

        private Dictionary<Guid, int> itemsInBasket;
        [NotMapped]
        public Dictionary<Guid, int> ItemsInBasket { get=>itemsInBasket; set => itemsInBasket = value; }

        public string ItemInBasketDB
        {
            get => JsonConvert.SerializeObject(itemsInBasket);
            set => itemsInBasket = JsonConvert.DeserializeObject<Dictionary<Guid, int>>(value);
        }

        public ShoppingBasket()
        {
            ShoppingBasketId = Guid.NewGuid();
        }

        public ShoppingBasket(Guid storeId)
        {
            storeID = storeId;
            itemsInBasket = new Dictionary<Guid, int>();

            ShoppingBasketId = Guid.NewGuid();
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ShoppingBasket))
            {
                return false;
            }
            ShoppingBasket other = (ShoppingBasket)obj;
            return storeID == other.StoreID;
        }
        public override int GetHashCode()
        {
            return this.storeID.GetHashCode();
        }

        public override string ToString()
        {
            string output = $"store ID: {storeID} with the items: \n";
            foreach (Guid item in itemsInBasket.Keys)
                output += $"    item ID: {item} with quantity: {itemsInBasket[item]}\n";
            return output;
        }
        
        public void AddItem(Guid itemID, int quantity)
        {
            if (quantity < 0)
                throw new Exception("cant add item with negative quantity");
            if (itemsInBasket.ContainsKey(itemID))
            {
                itemsInBasket[itemID] += quantity;
            }
            else
            {
                itemsInBasket.Add(itemID, quantity);
            }
        }
        
        public void RemoveItem(Guid itemID)
        {
            bool result = itemsInBasket.Remove(itemID);
            if(!result)
            {
                throw new Exception($"ItemID {itemID} not in basket");
            }
        }
        
        public void EditItem(Guid itemId, int quantity)
        {
            if (quantity < 0)
                throw new Exception("cant edit quantity with negative value");
            if (quantity == 0)
                RemoveItem(itemId);
            else if (itemsInBasket.ContainsKey(itemId))
                itemsInBasket[itemId] = quantity;
            else 
                throw new Exception("cant edit quantity of item that is not in the basket");
        }

        public int GetItemQuantity(Guid itemId)
        {
            if (itemsInBasket.ContainsKey(itemId))
                return itemsInBasket[itemId];
            throw new Exception("Item id "+ itemId +" is not in store id "+ storeID+ " shopping basket");
        }

        public void AddBasket(ShoppingBasket sb)
        {
            foreach (Guid id in sb.ItemsInBasket.Keys)
            {
                if (itemsInBasket.ContainsKey(id))
                    itemsInBasket[id] += sb.ItemsInBasket[id];
                else
                    itemsInBasket.Add(id,sb.itemsInBasket[id]);
            }
        }
    }
}