using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        private ConcurrentDictionary<Guid, Store> stores;
        private bool _isTSInitialized;
        private static Orders _orders;

        public StoreFacade()
        {
            stores = new ConcurrentDictionary<Guid, Store>();
            _orders = Orders.Instance;
        }

        public StoreFacade(ConcurrentDictionary<Guid, Store> stores)
        {
            this.stores = stores;
            _orders = Orders.Instance;
        }

        public Guid OpenNewStore(string storeName)
        {
            IsTsInitialized();
            if (storeName.Length == 0)
                throw new Exception("Store name can not be empty");
            Store store = new Store(storeName);
            stores.TryAdd(store.StoreID, store);
            Logger.Instance.Info(store.StoreID,nameof(StoreFacade)+": "+nameof(OpenNewStore)+"store " + storeName + " opened.");
            return store.StoreID;
        }

        public void CloseStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].Active = false;
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(CloseStore)+"store " + stores[storeID].StoreName + " closed.");
        }
        public void ReopenStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].Active = true;
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(ReopenStore)+"store " + stores[storeID].StoreName + " reopen.");
        }
        public void DeleteStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores.TryRemove(storeID, out var store);
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(DeleteStore)+"store " + store.StoreName + " deleted.");
        }
        
        public List<Order> GetStorePurchases(Guid storeID)
        {
            IsTsInitialized();
            return _orders.GetOrdersByStoreId(storeID);
        }
        public Dictionary<Guid, List<Order>> GetAllStorePurchases()
        {
            IsTsInitialized();
            return _orders.GetStoreOrders();
        }
        public void PurchaseItems(string storeName, List<string> itemsName)
        {
            IsTsInitialized();
            throw new System.NotImplementedException();
        }

        public List<Store> GetAllStoreInfo()
        {
            IsTsInitialized();
            return stores.Values.ToList();
        }

        public Guid AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(AddItemToStore)+" added to store "+ storeID + "- "+itemName +" form category "+itemCategory + ": "+itemPrice+"X"+quantity);
            return stores[storeID].AddItem(itemName, itemCategory, itemPrice, quantity);
        }

        public void RemoveItemFromStore(Guid storeID, Guid itemId)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].RemoveItem(itemId);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(RemoveItemFromStore)+" removed from store "+ storeID + "- "+itemId);
        }
        
        public void EditItemName(Guid storeID, Guid itemID, string name)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemName(itemID, name);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemName)+" edited item from store "+ storeID + "- "+storeID + "- "+name);
        }
        
        public void EditItemCategory(Guid storeID, Guid itemID, string category)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemCategory(itemID, category);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemCategory)+" edited category from store "+ storeID + "- "+storeID + "- "+category);

        }
        public void EditItemPrice(Guid storeID, Guid itemID, int price)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemPrice(itemID, price);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemPrice)+" edited price from store "+ storeID + "- "+storeID + "- "+price);

        }
        public void EditItemQuantity(Guid storeID, Guid itemID, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemQuantity(itemID, quantity);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemQuantity)+" edited quantity from store "+ storeID + "- "+storeID + "- "+quantity);

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
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(GetItemsByName));

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
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(GetItemsByCategory));
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
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(GetItemsByKeysWord));
            return allItems;
        }
        public void CleanUp()
        {
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

        private void IsStoreExist(Guid storeID)
        {
            if (!stores.ContainsKey(storeID))
                throw new Exception("Store with this id does not exist");
        }
            

        public void WriteItemReview(Guid userID, Guid storeID, Guid itemID, string reviewText)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            Store store = stores[storeID];
            
            bool foundUserOrder = false;
            foreach (Order order in _orders.GetOrdersByUserId(userID))
            {
                if (order.UserID.Equals(userID))
                    foundUserOrder = true;
            }
            if (!foundUserOrder)
                throw new Exception("user with id:" + userID + "tried writing review to item: " + itemID + " which he did not purchase before");
            Logger.Instance.Info(userID, nameof(StoreFacade)+": "+nameof(WriteItemReview) + userID +" write review to store "+storeID+" on "+itemID+"- "+ reviewText);
            store.WriteItemReview(userID, itemID, reviewText);
        }
        public ConcurrentDictionary<Guid, List<string>> GetItemReviews(Guid storeID, Guid itemID)
        {
            IsStoreExist(storeID);
            Store store = stores[storeID];
            return store.getItemsReviews(itemID);
        }

        public void AddItemToCart(Guid storeID, Guid itemID, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].AddItemToCart(itemID, quantity);
        }
    }
}
