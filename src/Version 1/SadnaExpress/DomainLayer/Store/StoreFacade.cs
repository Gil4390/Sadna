using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        private ConcurrentDictionary<Guid, Store> stores;
        public StoreFacade()
        {
            stores = new ConcurrentDictionary<Guid, Store>();
        }
        
        public Store GetStoreByID(Guid id)
        {
            foreach (Store store in stores.Values) {
                if (store.StoreId == id)
                    return store;
            }

            return null;
        }
        public Guid OpenNewStore(string storeName)
        {
            if(storeName.Length == 0)
                Logger.Instance.Error("Store name can not be empty");
            Store store = new Store(storeName);
            stores.TryAdd(store.StoreID, store);
            Logger.Instance.Info("store " + storeName + " opened.");
            return store.StoreID;
        }

        public void CloseStore(int id , Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            //
            store.Active = false;
            Logger.Instance.Info("store " + store.getName() + " closed.");
        }
        public void ReopenStore(int id , Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            //
            store.Active = true;
            Logger.Instance.Info("store " + store.getName() + " reopen.");
        }
        public void DeleteStore(int id , Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            stores.TryRemove(store.StoreID, out store);
            Logger.Instance.Info("store " + store.getName() + " deleted.");
        }
        
        public void PurchaseItems(string storeName, List<string> itemsName)
        {
            throw new System.NotImplementedException();
        }

        public void GetStoreHistory(string storeName)
        {
            throw new System.NotImplementedException();
        }

        public void AddItem(string storeName, string itemName, string category, int price)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItem(string storeName, string itemName, string category, int price)
        {
            throw new System.NotImplementedException();
        }

        public void EditItemCategory(string storeName, string itemName, string category)
        {
            throw new System.NotImplementedException();
        }

        public void EditItemPrice(string storeName, string itemName, int price)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByName(string itemName)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByCategory(string category)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByKeysWord(string keyWords)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByPrices(int minPrice, int maxPrice)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByItemRating(int rating)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> GetItemsByStoreRating(int rating)
        {
            throw new System.NotImplementedException();
        }

        public void ReviewItem(string storeName, string itemName, int rating)
        {
            throw new System.NotImplementedException();
        }

        public void CleanUp()
        {
           
        }
        public ConcurrentDictionary<Guid, Store> GetStores()
        {
            return stores;
        }
    }
}