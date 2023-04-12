using System;
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
        
        public void AddItemToInventory(Item item, int quantity)
        {
            bool result = this.items_quantity.TryAdd(item, quantity);
            if (result.Equals(false))
                throw new Exception("adding item failed");
        }


        public void RemoveItem(Item item)
        {
            int removed = 0;
            bool result = this.items_quantity.TryRemove(item, out removed);
            if (result.Equals(false))
                throw new Exception("removing item failed");
        }


        public void AddQuantity(Item item, int quantity)
        {
            if (this.items_quantity.ContainsKey(item))
                this.items_quantity[item] += quantity;
            else
                throw new Exception("Adding quantity failed, item not found");  
            
        }

        public void RemoveQuantity(Item item, int quantity)
        {
            if (this.items_quantity.ContainsKey(item))
                this.items_quantity[item] -= quantity;
            else
                throw new Exception("Removing quantity failed, item not found");
        }



        public void EditItemName(int itemId, string name)
        {
            Item item = getItemById(itemId);
            if (item.Equals(null))
                throw new Exception("Edit item failed, item not found");
            if (name.Equals(""))
                throw new Exception("Edit item failed, item name cant be empty");
            if (ItemExistsByName(name))
                throw new Exception("Edit item failed, item name cant be edited to a name that belongs to another item in the store");
            item.setName(name);
        }

        public void EditItemCategory(int itemId, string category)
        {
            Item item = getItemById(itemId);
            if (item.Equals(null))
                throw new Exception("Edit item failed, item not found");
            if (category.Equals(""))
                throw new Exception("Edit item failed, item category cant be empty");
            item.setCategory(category);
        }

        public void EditItemPrice(int itemId, double price)
        {
            Item item = getItemById(itemId);
            if (item.Equals(null))
                throw new Exception("Edit item failed, item not found");
            if (price < 0)
                throw new Exception("Edit item failed, item price cant be negative");
            item.setPrice(price);
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


        public void setItemRating(int id, int rating)
        {
            Item item = getItemById(id);
            if (!item.Equals(null))
                item.setRating(rating);
        }


        // add new Item to store, if item exists with the same name return false
        public void AddItem(string name, string category, double price, int quantity)
        {
            if (ItemExistsByName(name))
            {
                throw new Exception("can't add item to the store with a name that belongs to another item");
            }
            Item newItem = new Item(name, category, price);
            AddItemToInventory(newItem, quantity);
        }

    }
}