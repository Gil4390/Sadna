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
        public bool addItem(string name, string category, double price, int quantity)
        {
            if (this.itemsInventory.ItemExistsByName(name))
            {
                return false;
            }
            Item newItem = new Item(name, category, price);
            
            this.itemsInventory.AddItem(newItem, quantity);
            return true;
        }

        


        public bool AddQuantity(int itemID, int addedQuantity)
        {
            bool result = this.itemsInventory.AddQuantity(this.itemsInventory.getItemById(itemID), addedQuantity);
            if (!result)
            {
                Logger.Instance.Error("Item removal failed (Item not Found)");
                return false;
            }
            return true;

        }

        public bool RemoveQuantity(int itemId, int removedQuantity)
        {
            bool result = this.itemsInventory.RemoveQuantity(this.itemsInventory.getItemById(itemId), removedQuantity);
            if (!result)
            {
                Logger.Instance.Error("Item removal failed (Item not Found)");
                return false;
            }
            return true;
        }

        public bool EditItemName(int itemId, string name)
        {
            Item item = this.itemsInventory.getItemById(itemId);
            if (item.Equals(null))
                return false;
            if (name.Equals(""))
                return false;
            if (this.itemsInventory.ItemExistsByName(name))
                return false;
            item.setName(name);
            return true;
        }

        public bool EditItemCategory(int itemId, string category)
        {
            Item item = this.itemsInventory.getItemById(itemId);
            if (item.Equals(null))
                return false;
            if (category.Equals(""))
                return false;
            item.setCategory(category);
            return true;
        }

        public bool EditItemPrice(int itemId, double price)
        {
            Item item = this.itemsInventory.getItemById(itemId);
            if (item.Equals(null))
                return false;
            if (price < 0)
                return false;
            item.setPrice(price);
            return true;
        }

        public bool RemoveItemById(int itemId)
        {
            return this.itemsInventory.RemoveItem(this.itemsInventory.getItemById(itemId));
        }

        public bool RemoveItemByName(string itemName)
        {
            return this.itemsInventory.RemoveItem(this.itemsInventory.getItemByName(itemName));
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
            Item item = this.itemsInventory.getItemById(id);
            if(!item.Equals(null))
                item.setRating(rating);
        }



        public bool Active
        {
            get => active;
            set => active = value;
        }
    }
}