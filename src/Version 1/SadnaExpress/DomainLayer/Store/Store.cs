using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer.Store
{
    public class Store
    {
        private string storeName;
        public string StoreName {get => storeName; set => storeName = value;}
        public Inventory itemsInventory;
        private Guid storeID;
        public Guid StoreID {get=>storeID;}
        //private Policy policy; // not for this version

        private bool active;
        public bool Active { get => active; set => active = value; }

        private int storeRating;
        public int StoreRating {  get => storeRating ; set => StoreRating = value; }

        public Store(string name) {
            storeName = name;
            itemsInventory = new Inventory();
            storeRating = 0;
            storeID = Guid.NewGuid();
            active = true;
            //this.policy = new Policy();
        }

        public Item GetItemsByName(string itemName, int minPrice, int maxPrice, string category, int ratingItem)
        {
            return itemsInventory.GetItemByName(itemName, minPrice, maxPrice,category, ratingItem);
        }
        public List<Item> GetItemsByCategory(string category, int minPrice, int maxPrice, int ratingItem)
        {
            return itemsInventory.GetItemsByCategory(category, minPrice, maxPrice, ratingItem);
        }
        public List<Item> GetItemsByKeysWord(string keyWords, int minPrice, int maxPrice, int ratingItem, string category)
        {
            return itemsInventory.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category);
        }
        public Guid AddItem(string name, string category, double price, int quantity)
        {
            return itemsInventory.AddItem(name, category, price, quantity);
        }
        public void RemoveItem(Guid itemID)
        {
            itemsInventory.RemoveItem(itemID);
        }
        public void EditItemPrice(Guid itemID, int price)
        {
            itemsInventory.EditItemPrice(itemID, price);
        }
        public void EditItemName(Guid itemID, string name)
        {
            itemsInventory.EditItemName(itemID, name);
        }
        public void EditItemCategory(Guid itemID, string category)
        {
            itemsInventory.EditItemCategory(itemID, category);
        }

        internal void WriteItemReview(Guid userID, Guid itemID, string reviewText)
        {
            itemsInventory.AddReviewToItem(userID, reviewText, itemID);
        }
        internal ConcurrentDictionary<Guid, List<string>> getItemsReviews(Guid itemID)
        {
            return itemsInventory.getItemReviews(itemID);
        }

        public void EditItemQuantity(Guid itemID, int quantity)
        {
            itemsInventory.EditItemQuantity(itemID, quantity);
        }

        public void AddItemToCart(Guid itemID, int quantity)
        {
            itemsInventory.AddItemToCart(itemID, quantity);
        }

        public double PurchaseCart(Dictionary<Guid, int> items)
        {
            return itemsInventory.PurchaseCart(items);
        }
    }
}
