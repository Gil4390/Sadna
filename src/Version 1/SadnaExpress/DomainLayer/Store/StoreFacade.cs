using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        private ConcurrentDictionary<Guid, Store> stores;
        private ConcurrentDictionary<Guid, LinkedList<Order>> storeOrders;

        public StoreFacade(ISupplierService supplierService=null)
        {
            stores = new ConcurrentDictionary<Guid, Store>();
            storeOrders = new ConcurrentDictionary<Guid, LinkedList<Order>>();
        }
        
        public Store GetStoreByID(Guid id)
        {
            foreach (Store store in stores.Values) {
                if (store.StoreID.Equals(id))
                    return store;
            }
            return null;
        }
        public Guid OpenNewStore(string storeName)
        {
            if(storeName.Length == 0)
                throw new Exception("Store name can not be empty");
            Store store = new Store(storeName);
            stores.TryAdd(store.StoreID, store);
            Logger.Instance.Info("store " + storeName + " opened.");
            return store.StoreID;
        }

        public void CloseStore(Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            //
            store.Active = false;
            Logger.Instance.Info("store " + store.StoreName + " closed.");
        }
        public void ReopenStore(Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            //
            store.Active = true;
            Logger.Instance.Info("store " + store.StoreName + " reopen.");
        }
        public void DeleteStore(Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            stores.TryRemove(store.StoreID, out store);
            Logger.Instance.Info("store " + store.StoreName + " deleted.");
        }
        
        public LinkedList<Order> GetStorePurchases(Guid storeId)
        {
            if (!storeOrders.ContainsKey(storeId))
                throw new Exception("Store with this id does not exist");
            Logger.Instance.Info("store " + GetStoreByID(storeId).StoreName + " got his purchases info.");
            return storeOrders[storeId];
        }
        public void PurchaseItems(string storeName, List<string> itemsName)
        {   
            throw new System.NotImplementedException();

        }

        public Store GetStoreById(Guid storeId)
        {
            if(stores.ContainsKey(storeId))
                return stores[storeId];
            throw new Exception("there is no store with this id");
        }

        public void GetAllStoreInfo(string storeName)
        {
            throw new System.NotImplementedException();

        }

        public void AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            GetStoreById(storeID).addItem(itemName, itemCategory, itemPrice, quantity);
        }

        public void RemoveItemFromStore(Guid storeID, int itemId)
        {
            GetStoreById(storeID).RemoveItemById(itemId);
        }

        public void EditItemCategory(string storeName, string itemName, string category)
        {
            throw new System.NotImplementedException();
        }

        public void EditItemPrice(string storeName, string itemName, int price)
        {
            throw new System.NotImplementedException();
        }
        
        public List<Item> GetItemsByName(string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                if (ratingStore != -1 && store.StoreRating != ratingStore)
                    continue;
                Item item = store.GetItemsByName(itemName, minPrice, maxPrice, category, ratingItem);
                if (item != null)
                    allItems.Add(item);
            }
            return allItems;
        }
        public List<Item> GetItemsByCategory(string category, int minPrice, int maxPrice, int ratingItem, int ratingStore)
        {
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                if (ratingStore != -1 && store.StoreRating != ratingStore)
                    continue;
                allItems.AddRange(store.GetItemsByCategory(category, minPrice, maxPrice, ratingItem));
            }
            return allItems;
        }
        public List<Item> GetItemsByKeysWord(string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                if (ratingStore != -1 && store.StoreRating != ratingStore)
                    continue;
                allItems.AddRange(store.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category));
            }
            return allItems;
        }
        public void WriteReview(string storeName, string itemName, int rating)
        {
            throw new System.NotImplementedException();
        }

        public void CleanUp()
        {
           storeOrders.Clear();
           stores.Clear();
        }

        public ConcurrentDictionary<Guid, Store> GetStores()
        {
            return stores;
        }
    }
}