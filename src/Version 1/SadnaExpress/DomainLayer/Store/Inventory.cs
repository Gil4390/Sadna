using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class Inventory
    {
        private Dictionary<Item, int> items_quantity;


        public Inventory()
        {
            this.items_quantity = new Dictionary<Item, int>();
        }

        //getters

        public Dictionary<Item, int> getItems()
        {
            return this.items_quantity;
        }
        
        public bool AddItem(Item item, int quantity)
        {
            if (this.items_quantity.ContainsKey(item))
                return false;
            this.items_quantity.Add(item, quantity);
            return true;
        }


        public bool RemoveItem(Item item)
        {
            return this.items_quantity.Remove(item);
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