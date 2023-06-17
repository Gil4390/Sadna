using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using SadnaExpress.DataLayer;

namespace SadnaExpress.DomainLayer.Store
{
    public class Inventory
    {
        [Key]
        public Guid InventoryID { get; set; }
        
        [NotMapped]
        public ConcurrentDictionary<Item, int> items_quantity { get; set; }

        public Guid StoreID { get; set; }

        [NotMapped]
        public ConcurrentDictionary<Guid, int> items_quantityHelper
        {
            get
            {
                ConcurrentDictionary<Guid, int> item_quantityH = new ConcurrentDictionary<Guid, int>();
                foreach (Item it in items_quantity.Keys)
                    item_quantityH.TryAdd(it.ItemID, items_quantity[it]);
                return item_quantityH;
            }
            set
            {

            }
        }
        [NotMapped]
        public string Items_quantityJson
        {
            get => JsonConvert.SerializeObject(items_quantityHelper);
            set => items_quantityHelper = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, int>>(value);
        }

        public string Items_quantityDB
        {
            get; set;
        }        

        public Inventory()
        {
            items_quantity = new ConcurrentDictionary<Item, int>();
            InventoryID = Guid.NewGuid();
        }

        public bool Equals(ConcurrentDictionary<Item, int> newItems_quantity)
        {
            return items_quantity.Count == newItems_quantity.Count && !items_quantity.Except(newItems_quantity).Any();
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

        public Item GetItemByName(string itemName)
        {
            foreach (Item item in items_quantity.Keys)
                if (item.Name.ToLower().Equals(itemName.ToLower()))
                    return item;
            throw new Exception($"the item name {itemName} not exist!");
        }
      
        public List<Item> GetItemsByKeysWord(string keyWords, int minPrice, int maxPrice, int ratingItem, string category)
        {
            List<Item> items = new List<Item>();
            foreach (Item item in items_quantity.Keys)
            {
                if (item.Name.ToLower().Contains(keyWords.ToLower()) && item.Price >= minPrice && item.Price <= maxPrice)
                {
                    if (ratingItem != -1 && item.Rating < ratingItem)
                        continue;
                    if (category != null && item.Category.ToLower().Contains(category.ToLower()) ==false)
                        continue;
                    items.Add(item);
                }
            }
            return items;
        }
        public Guid AddItem(string name, string category, double price, int quantity)
        {
            if (quantity < 0)
                throw new Exception("quantity can't be smaller than 0");
            String internedKey = String.Intern(name.ToLower());

            lock (internedKey)
            {
                if (itemExistsByName(name))
                {
                    throw new Exception("can't add item to the store with a name that belongs to another item");
                }
                Item newItem = new Item(name, category, price);
                newItem.InventoryID = this.InventoryID;
                items_quantity.TryAdd(newItem, quantity);

                // DBhandler add item in database
                DBHandler.Instance.AddItem(newItem);

                return newItem.ItemID;
            }
        }
        public void RemoveItem(Guid itemID)
        {
            int outItem;
            if (!items_quantity.TryRemove(GetItemById(itemID), out outItem))
                throw new Exception("The item not exist");
            DBHandler.Instance.RemoveItem(itemID);
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
            if (item.Name != name)
            {
                lock (item)
                {
                    if (name.Equals(""))
                        throw new Exception("Edit item failed, item name cant be empty");
                    if (itemExistsByName(name))
                        throw new Exception(
                            "Edit item failed, item name cant be edited to a name that belongs to another item in the store");
                    item.Name = name;
                    //DBHandler.Instance.UpdateItem(item);
                }
            }
        }

        public void EditItemCategory(Guid itemID, string category)
        {
            Item item = GetItemById(itemID);
            if (item.Category != category)
            {
                lock (item)
                {
                    if (category.Equals(""))
                        throw new Exception("Edit item failed, item category cant be empty");
                    item.Category = category;
                    //DBHandler.Instance.UpdateItem(item);
                }
            }
        }
        public void EditItemPrice(Guid itemId, double price)
        {
            Item item = GetItemById(itemId);
            if (item.Price != price)
            {
                lock (item)
                {
                    if (price < 0)
                        throw new Exception("Edit item failed, item price cant be negative");
                    item.Price = price;
                    //DBHandler.Instance.UpdateItem(item);
                }
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

        public double PurchaseCart(Dictionary<Guid, int> items, Dictionary<Item, KeyValuePair<double, DateTime>> itemAfterDiscount, ref List<ItemForOrder> itemForOrders, Guid storeID , string storeName, string email)
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
                        for (int i = 0; i < items[itemID]; i++)
                        {
                            ItemForOrder ifo;
                            if (itemAfterDiscount.ContainsKey(item))
                                ifo = new ItemForOrder(item, itemAfterDiscount[item].Key, storeID, email, storeName);
                            else
                                ifo = new ItemForOrder(item, storeID, email, storeName);
                            itemForOrders.Add(ifo);
                        }
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