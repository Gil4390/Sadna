using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.DomainLayer.Store
{
    public class Store
    {
        private string name;
        private List<Inventory> itemsInventory;
        private Guid storeID;
        public Guid StoreID {get=>storeID;}
        private Policy policy;
        //username, rating 1-5
        private Dictionary<string, int> storeReview;
        // maybe need to add store discount
        public Store(string name) {
            this.name = name;
            this.itemsInventory = new List<Inventory>();
            this.policy = new Policy();
            this.storeReview = new Dictionary<string, int>();
            storeID = new Guid();
        }

        //getters
        public string getName() {
            return this.name;
        }

        public Policy getPolicy() {
            return this.policy;
        }

        

        public List<Inventory> getItemsInventory()
        {
            return this.itemsInventory;
        }

        


        // add new Item to store, if item exists with the same name return false
        public bool addItem(string name, string category, double price, int in_stock, Policy policy)
        {
            Inventory exists = getItem(name);
            if (exists != null)
            {
                return false;
            }
            Item newItem = new Item(name, category, price);
            Inventory inv = new Inventory(newItem, in_stock, price, policy, this);
            this.itemsInventory.Add(inv);
            return true;
        }

        public Inventory getItem(String name)
        {
            foreach (Inventory inv in itemsInventory)
            {
                if (inv.getName() == name)
                {
                    return inv;
                }
            }
            return null;
        }

        public bool deleteItem(string name)
        {
            foreach (Inventory inv in itemsInventory)
            {
                if (inv.getName() == name)
                {
                    itemsInventory.Remove(inv);
                    return true;
                }
            }
            Logger.Instance.Error("Item removal failed (Item not Found)");
            return false;
        }
        
        public bool updateItem(string name, string newName, string newCategory, double newPrice, int newIn_stock, Policy newPolicy)
        {
            Inventory inventoryitem = getItem(name);

            if(newName.Equals(""))
            {
                return false;
            }
            if (newCategory.Equals(""))
            {
                return false;
            }

            if(newPrice <= 0)
            {
                return false;
            }

            if (newIn_stock < 0)
            {
                return false;
            }

            inventoryitem.setPrice(newPrice);
            inventoryitem.setInStock(newIn_stock);
            inventoryitem.setPolicy(newPolicy);
            inventoryitem.setItem(new Item(newName, newCategory, newPrice));

            return true;
        }


        // this function handles stock updates for the store after a purchase
        public bool updateStockAfterPurchase(Inventory inv, int stock)
        {
            int newStock = inv.getInStock() - stock;
            if ( newStock < 0)
            {
                Logger.Instance.Error("Not Enough Items in stock to complete this purhcase, please try later!");
            }
            else
            {
                inv.setInStock(newStock);
                return true;
            }
            return false;
        }


        public void addStoreReview(string username, int rating)
        {
            this.storeReview.Add(username, rating);

        }


    }
}