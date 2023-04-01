using ConsoleApp1.DomainLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public class Store
    {
        private string name;
        private List<Inventory> itemsInvetory;
        private StoreFounder storeFounder;
        private List<StoreOwner> storeOwners;
        private Policy policy;

        //                 username, rating 1-5
        private Dictionary<string, int> storeReview;

        // maybe need to add store discount


        public Store(string name, StoreFounder storeFounder)
        {
            this.name = name;
            this.storeFounder = storeFounder;
            this.itemsInvetory = new List<Inventory>();
            this.storeOwners = new List<StoreOwner>();
            
            //this.policy = new Policy();
            this.storeReview = new Dictionary<string, int>();
        }

        //getters

        public StoreFounder GetStoreFounder()
        {
            return this.storeFounder;
        }
        public string getName()
        {
            return this.name;
        }


        public Policy getPolicy()
        {
            return this.policy;
        }

        public List<StoreOwner> GetOwners()
        {
            return this.storeOwners;
        }

        public List<Inventory> getItemsInvetory()
        {
            return this.itemsInvetory;
        }


        // adds new owner to the store
        public void addOwner(StoreOwner newOwner)
        {
            this.storeOwners.Add(newOwner);
        }

        // remove owner
        public void removeOwner(StoreOwner owner)
        {
            this.storeOwners.Remove(owner);
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
            this.itemsInvetory.Add(inv);
            return true;
        }

        public Inventory getItem(String name)
        {
            foreach (Inventory inv in itemsInvetory)
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
            foreach (Inventory inv in itemsInvetory)
            {
                if (inv.getName() == name)
                {
                    itemsInvetory.Remove(inv);
                    return true;
                }
            }
            throw new SadnaExpressException("Item removal failed (Item not Found)", "Store", "deleteItem");
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
                throw new SadnaExpressException("Not Enough Items in stock to complete this purhcase, please try later!", "Store", "updateStockAfterPurchase");
            }
            else
            {
                inv.setInStock(newStock);
                return true;
            }
        }


        public void addStoreReview(string username, int rating)
        {
            this.storeReview.Add(username, rating);

        }


    }
}