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
        private bool _isTSInitialized;

        public StoreFacade()
        {
            stores = new ConcurrentDictionary<Guid, Store>();
            storeOrders = new ConcurrentDictionary<Guid, LinkedList<Order>>();
        }
        
        public Guid OpenNewStore(string storeName)
        {
            IsTsInitialized();
            if (storeName.Length == 0)
                throw new Exception("Store name can not be empty");
            Store store = new Store(storeName);
            stores.TryAdd(store.StoreID, store);
            Logger.Instance.Info("store " + storeName + " opened.");
            return store.StoreID;
        }

        public void CloseStore(Guid storeId)
        {
            IsTsInitialized();
            if (!stores.ContainsKey(storeId))
                Logger.Instance.Error("there is no store with this ID");
            stores[storeId].Active = false;
            Logger.Instance.Info("store " + stores[storeId].StoreName + " closed.");
        }
        public void ReopenStore(Guid storeId)
        {
            IsTsInitialized();
            if (!stores.ContainsKey(storeId))
            stores[storeId].Active = true;
            Logger.Instance.Info("store " + stores[storeId].StoreName + " reopen.");                
            Logger.Instance.Error("there is no store with this name");
        }
        public void DeleteStore(Guid storeId)
        {
            IsTsInitialized();
            if (!stores.ContainsKey(storeId))
                Logger.Instance.Error("there is no store with this name");
            stores.TryRemove(storeId, out var store);
            Logger.Instance.Info("store " + store.StoreName + " deleted.");
        }
        
        public LinkedList<Order> GetStorePurchases(Guid storeId)
        {
            IsTsInitialized();
            if (!storeOrders.ContainsKey(storeId))
                throw new Exception("Store with this id does not exist");
            Logger.Instance.Info("store " + stores[storeId].StoreName + " got his purchases info.");
            return storeOrders[storeId];
        }
        public void PurchaseItems(string storeName, List<string> itemsName)
        {
            IsTsInitialized();
            throw new System.NotImplementedException();

        }

        public Store GetStoreById(Guid storeId)
        {
            IsTsInitialized();
            if (stores.ContainsKey(storeId))
                return stores[storeId];
            throw new Exception("there is no store with this id");
        }

        public void GetAllStoreInfo(string storeName)
        {
            IsTsInitialized();
            throw new System.NotImplementedException();

        }

        public void AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            IsTsInitialized();
            GetStoreById(storeID).addItem(itemName, itemCategory, itemPrice, quantity);
        }

        public void RemoveItemFromStore(Guid storeID, int itemId)
        {
            IsTsInitialized();
            GetStoreById(storeID).RemoveItemById(itemId);
        }

        public void EditItemCategory(string storeName, string itemName, string category)
        {
            IsTsInitialized();
            throw new System.NotImplementedException();
        }

        public void EditItemPrice(string storeName, string itemName, int price)
        {
            IsTsInitialized();
            throw new System.NotImplementedException();
        }
        
        public List<Item> GetItemsByName(string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            IsTsInitialized();
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                if (!store.Active)
                    continue;
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
            IsTsInitialized();
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                if (!store.Active)
                    continue;
                if (ratingStore != -1 && store.StoreRating != ratingStore)
                    continue;
                allItems.AddRange(store.GetItemsByCategory(category, minPrice, maxPrice, ratingItem));
            }
            return allItems;
        }
        public List<Item> GetItemsByKeysWord(string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            IsTsInitialized();
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                if (!store.Active)
                    continue;
                if (ratingStore != -1 && store.StoreRating != ratingStore)
                    continue;
                allItems.AddRange(store.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category));
            }
            return allItems;
        }
        public void WriteReview(string storeName, string itemName, int rating)
        {
            IsTsInitialized();
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

        public void SetIsSystemInitialize(bool isInitialize)
        {
            _isTSInitialized = isInitialize;
        }

        private void IsTsInitialized()
        {
            if (_isTSInitialized == false)
                throw new Exception("Cannot preform any action because system trading is closed");
        }
    }
}