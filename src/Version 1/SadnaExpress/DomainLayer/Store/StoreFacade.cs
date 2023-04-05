using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        private ConcurrentBag<Store> stores;
        public StoreFacade()
        {
            stores = new ConcurrentBag<Store>();
        }

        public Store getStoreByName(string name)
        {
            foreach (Store store in stores) {
                if (store.getName().Equals(name))
                    return store;
            }

            return null;
        }
        public void OpenNewStore(string storeName)
        {
            if(storeName.Length == 0)
                throw new SadnaException("Store name can not be empty", "StoreFacade", "OpenNewStore");
            if (getStoreByName(storeName) == null) {
                Store store = new Store(storeName);
                stores.Add(store);
                Logger.Instance.Info("store " + storeName + " opened.");
            }
            else
                throw new SadnaException("There is already store with the same name", "StoreFacade", "OpenNewStore");
        }

        public void CloseStore(string storeName)
        {
            Store store = getStoreByName(storeName);
            if (store == null)
                throw new SadnaException("there is no store with this name", "StoreFacade", "CloseStore");
            stores.TryTake(out store);
            Logger.Instance.Info("store " + storeName + " closed.");
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
    }
}