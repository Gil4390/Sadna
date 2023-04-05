using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        private LinkedList<Store> stores;
        public StoreFacade()
        {
            stores = new LinkedList<Store>();
        }

        public Store getStoreByName(string name)
        {
            foreach (Store store in stores) {
                if (store.getName().Equals(name))
                    return store;
            }

            return null;
        }
        public bool OpenNewStore(string storeName)
        {
            if (getStoreByName(storeName) == null)
            {
                Store store = new Store(storeName);
                stores.AddLast(store);
                Logger.Info("store " + storeName + " opened.");
                return true;
            }

            return false;
        }

        public bool CloseStore(string storeName)
        {
            Store store = getStoreByName(storeName);
            if (store == null)
                return false;
            stores.Remove(store);
            Logger.Info("store " + storeName + " closed.");
            return true;
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