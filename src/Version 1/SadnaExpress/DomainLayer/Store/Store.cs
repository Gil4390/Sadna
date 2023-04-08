using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.User;
using static SadnaExpress.DomainLayer.Store.PurchasePolicy;

namespace SadnaExpress.DomainLayer.Store
{
    public class Store
    {
        private string name;
        private int founderId;
        private List<Inventory> itemsInventory;
        private Guid storeID;
        public Guid StoreID {get=>storeID;}

        private List<Policy> storePolicy;


        /// <summary>
        ///  for each owner id list of owners id he assigned after him
        /// </summary>
        Dictionary<int, List<string>> owners;

        /// <summary>
        ///  for each manager list of permissions he will do
        /// </summary>
        Dictionary<string, List<string>> managers;


        public Store(string name, int founderId) {
            this.name = name;
            this.itemsInventory = new List<Inventory>();

            storeID = new Guid();
            this.founderId = founderId;
            this.owners = new Dictionary<int, List<string>>();
            this.owners.Add(founderId, new List<string>());
            this.storePolicy = new List<Policy>();
            this.storePolicy.Add(new NoPurchasePolicy());
            //this.discount = new NoDiscount();
            // founder first one
        }

        //getters
        public string getName() {
            return this.name;
        }

        public List<Policy> getPolicy() {
            return this.storePolicy;
        }


        public Guid getId()
        {
            return this.storeID;
        }

        public List<Inventory> getItemsInventory()
        {
            return this.itemsInventory;
        }


        // add new Item to store, if item exists with the same name return false
        public bool addItem(string name, string category, double price, int in_stock, DiscountPolicy dPolicy)
        {
            Inventory exists = getItem(name);
            if (exists != null)
            {
                return false;
            }
            Item newItem = new Item(name, category, price);
            Inventory inv = new Inventory(newItem, in_stock, price, dPolicy);
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


        public bool AddStock(int id, int addedStock)
        {
            foreach (Inventory inv in itemsInventory)
            {
                if (inv.GetId() == id)
                {
                    inv.addInStock(addedStock);
                    return true;
                }
            }
            Logger.Instance.Error("Item removal failed (Item not Found)");
            return false;
        }

        public bool RemoveStock(int id, int removedStock)
        {
            foreach (Inventory inv in itemsInventory)
            {
                if (inv.GetId() == id)
                {
                    inv.removeInStock(removedStock);
                    return true;
                }
            }
            Logger.Instance.Error("Item removal failed (Item not Found)");
            return false;
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

            if (newName.Equals(""))
            {
                return false;
            }
            if (newCategory.Equals(""))
            {
                return false;
            }

            if (newPrice <= 0)
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

        public bool HasPermissionToAdd(int id)
        {
            if (id == this.founderId)
                return true;
            throw new NotImplementedException();
        }


        // need to implement owners functions


        // need to implement managers functions




    }
}