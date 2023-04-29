using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SadnaExpress.DomainLayer.Store
{
    public class Inventory
    {
        public ConcurrentDictionary<Item, int> items_quantity;
        
        public Inventory()
        {
            items_quantity = new ConcurrentDictionary<Item, int>();
        }
        
        // name is unique
        public Item GetItemByName(string itemName, int minPrice, int maxPrice, string category, int ratingItem)
        {
            foreach (Item item in items_quantity.Keys)
            {
                if (item.Name.ToLower().Equals(itemName.ToLower()) && item.Price >= minPrice && item.Price <= maxPrice)
                {
                    if (ratingItem != -1 && item.Rating != ratingItem)
                        break;
                    if (category != null && item.Category != category)
                        break;
                    return item;
                }
            }
            return null;
        }
        public List<Item> GetItemsByCategory(string category, int minPrice, int maxPrice, int ratingItem)
        {
            List<Item> items = new List<Item>();
            foreach (Item item in items_quantity.Keys)
            {
                if (item.Category.ToLower().Equals(category.ToLower()) && item.Price >= minPrice && item.Price <= maxPrice)
                {
                    if (ratingItem != -1 && item.Rating != ratingItem)
                        continue;
                    items.Add(item);
                }
            }
            return items;
        }
        public List<Item> GetItemsByKeysWord(string keyWords, int minPrice, int maxPrice, int ratingItem, string category)
        {
            List<Item> items = new List<Item>();
            foreach (Item item in items_quantity.Keys)
            {
                if (item.Name.ToLower().Contains(keyWords.ToLower()) && item.Price >= minPrice && item.Price <= maxPrice)
                {
                    if (ratingItem != -1 && item.Rating != ratingItem)
                        continue;
                    if (category != null && item.Category != category)
                        continue;
                    items.Add(item);
                }
            }
            return items;
        }
        public Guid AddItem(string name, string category, double price, int quantity)
        {
            String internedKey = String.Intern(name.ToLower());

            lock (internedKey)
            {
                if (itemExistsByName(name))
                {
                    throw new Exception("can't add item to the store with a name that belongs to another item");
                }
                Item newItem = new Item(name, category, price);
                items_quantity.TryAdd(newItem, quantity);
                return newItem.ItemID;
            }
        }
        public void RemoveItem(Guid itemID)
        {
            int outItem;
            items_quantity.TryRemove(GetItemById(itemID), out outItem);
        }
        
        public void EditItemQuantity(Guid itemID, int quantity)
        {
            Item item = GetItemById(itemID);
            lock (item)
            {
                if (items_quantity[item] + quantity < 0)
                    throw new Exception("Edit item quantity failed, item quantity cant be negative");
                items_quantity[item] += quantity;
            }
        }
        public void EditItemName(Guid itemID, string name)
        {
            Item item = GetItemById(itemID);
            lock (item)
            {
                if (name.Equals(""))
                    throw new Exception("Edit item failed, item name cant be empty");
                if (itemExistsByName(name))
                    throw new Exception(
                        "Edit item failed, item name cant be edited to a name that belongs to another item in the store");
                item.Name = name;
            }
        }
        public void EditItemCategory(Guid itemID, string category)
        {
            Item item = GetItemById(itemID);
            lock (item)
            {
                if (category.Equals(""))
                    throw new Exception("Edit item failed, item category cant be empty");
                item.Category = category;
            }
        }
        public void EditItemPrice(Guid itemId, double price)
        {
            Item item = GetItemById(itemId);
            lock(item)
            {
                if (price < 0)
                    throw new Exception("Edit item failed, item price cant be negative");
                item.Price = price;
            }
        }
        private bool itemExistsByName(string itemName)
        {
            foreach (Item item in this.items_quantity.Keys)
            {
                if (item.Name.ToLower()==itemName.ToLower())
                    return true;
            }
            return false;
        }
        public Item GetItemById(Guid itemId)
        {
            foreach (Item item in items_quantity.Keys)
            {
                if (item.ItemID.Equals(itemId))
                    return item;
            }
            throw new Exception("The item not exist");
        }

        public void AddItemToCart(Guid itemID, int quantity)
        {
            if (items_quantity[GetItemById(itemID)] < quantity)
                throw new Exception("You can't add to the cart, the quantity in the inventory not enough");
        }

        public double PurchaseCart(Dictionary<Guid, int> items, ref List<ItemForOrder> itemForOrders, Guid storeID)
        {
            Dictionary<Item, int> itemsUpdated = new Dictionary<Item, int>(); //items that the quantity already updated (need to be save in case of error)  
            try
            {
                double sum = 0;
                foreach (Guid itemID in items.Keys)
                {
                    Item item = GetItemById(itemID);
                    lock (item)
                    {
                        if (items_quantity[item] - items[itemID] < 0)
                            throw new Exception($"The item {item.Name} finished");
                        items_quantity[item] -= items[itemID];
                        itemForOrders.Add(new ItemForOrder(item, storeID));
                    }
                    sum += item.Price;
                    itemsUpdated.Add(item, items[itemID]);
                }
                return sum;
            }
            catch (Exception e)
            {
                foreach (Item item in itemsUpdated.Keys)
                    EditItemQuantity(item.ItemID, itemsUpdated[item]);
                throw e;
            }
        }
        public int GetItemByQuantity(Guid itemID)
        {
            return items_quantity[GetItemById(itemID)];
        }

        public bool ItemExist(Guid itemID)
        {
            foreach (Item item in items_quantity.Keys)
            {
                if(item.ItemID == itemID) return true;
            }
            return false;
        }
    }
}