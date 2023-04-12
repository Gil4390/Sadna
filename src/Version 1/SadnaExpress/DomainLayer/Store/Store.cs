using System;
using System.Collections.Generic;
using System.Diagnostics;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer.Store
{
    public class Store
    {
        private string storeName;
        private Inventory itemsInventory;
        private Guid storeID;
        public Guid StoreID {get=>storeID;}
        //private Policy policy; // not for this version

        private bool active;

        private int storeRating;
        
        public Store(string name) {
            this.storeName = name;
            this.itemsInventory = new Inventory();
            //this.policy = new Policy();
            this.storeRating = 0;
            storeID = Guid.NewGuid();
            active = true;
        }

        //getters
        public string getName() {
            return this.storeName;
        }

        public Guid StoreId
        {
            get => storeID;
            set => storeID = value;
        }


        public Inventory getItemsInventory()
        {
            return this.itemsInventory;
        }

        // add new Item to store, if item exists with the same name return false
        public void addItem(string name, string category, double price, int quantity)
        {
            this.itemsInventory.AddItem(name, category, price, quantity);
        }

        public void AddQuantity(int itemID, int addedQuantity)
        {
            this.itemsInventory.AddQuantity(this.itemsInventory.getItemById(itemID), addedQuantity);
        }

        public void RemoveQuantity(int itemId, int removedQuantity)
        {
            this.itemsInventory.RemoveQuantity(this.itemsInventory.getItemById(itemId), removedQuantity);
        }

        public void EditItemName(int itemId, string name)
        {
            this.itemsInventory.getItemById(itemId);
        }

        public void EditItemCategory(int itemId, string category)
        {
            this.itemsInventory.EditItemCategory(itemId,category);
        }

        public void EditItemPrice(int itemId, double price)
        {
            this.itemsInventory.EditItemPrice(itemId,price);
        }

        public void RemoveItemById(int itemId)
        {
            this.itemsInventory.RemoveItem(this.itemsInventory.getItemById(itemId));
        }

        public void RemoveItemByName(string itemName)
        {
            this.itemsInventory.RemoveItem(this.itemsInventory.getItemByName(itemName));
        }

        public int getStoreRating()
        {
            return this.storeRating;
        }

        public void setStoreRating(int rating)
        {
            this.storeRating = rating;
        }

        public void setItemRating(int id, int rating)
        {
            this.itemsInventory.setItemRating(id, rating);
        }

        public bool Active
        {
            get => active;
            set => active = value;
        }
    }
}