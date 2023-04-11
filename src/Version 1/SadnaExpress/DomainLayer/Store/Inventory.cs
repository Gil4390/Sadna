using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class Inventory
    {
        private ConcurrentDictionary<Item, int> items_quantity;


        public Inventory()
        {
            this.items_quantity = new ConcurrentDictionary<Item, int>();
        }

        //getters

        public ConcurrentDictionary<Item, int> getItems()
        {
            return this.items_quantity;
        }
        
        public bool AddItem(Item item, int quantity)
        {
            bool result = this.items_quantity.TryAdd(item, quantity);
            return result;
        }


        public bool RemoveItem(Item item)
        {
            int removed = 0;
            bool result = false;
            result = this.items_quantity.TryRemove(item, out removed);
            return result;
        }


        public bool AddQuantity(Item item, int quantity)
        {
            if (this.items_quantity.ContainsKey(item))
            {
                this.items_quantity[item] += quantity;
                return true;
            }
            return false;
        }

        public bool RemoveQuantity(Item item, int quantity)
        {
            if (this.items_quantity.ContainsKey(item))
            {
                this.items_quantity[item] -= quantity;
                return true;
            }
            return false;
        }

        

        public bool EditItem(Item item, string name, string category, double price)
        {
            if (this.items_quantity.ContainsKey(item))
            {
                item.setPrice(price);
                item.setName(name);
                item.setCategory(category);
                return true;
            }
            return false;
        }

        public bool ItemExistsById(int itemId)
        {
            foreach (Item item in this.items_quantity.Keys)
            {
                if (item.getId().Equals(itemId))
                    return true;
            }
            return false;

        }

        public bool ItemExistsByName(string itemName)
        {
            foreach (Item item in this.items_quantity.Keys)
            {
                if (item.getName().Equals(itemName))
                    return true;
            }
            return false;

        }

        public Item getItemByName(string itemName)
        {
            foreach (Item item in this.items_quantity.Keys)
            {
                if (item.getName().Equals(itemName))
                    return item;
            }
            return null;
        }

        public Item getItemById(int itemId)
        {
            foreach (Item item in this.items_quantity.Keys)
            {
                if (item.getId().Equals(itemId))
                    return item;
            }
            return null;
        }

        public int getItemQuantityById(int itemId)
        {
            Item item = getItemById(itemId);
            if (item == null)
                return -1;
            return this.items_quantity[item];
        }


    }
}