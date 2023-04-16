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
            Logger.Instance.Info("store " + storeName + " opened.");
            return store.StoreID;
        }

        public void CloseStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].Active = false;
            Logger.Instance.Info("store " + stores[storeID].StoreName + " closed.");
        }
        public void ReopenStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].Active = true;
            Logger.Instance.Info("store " + stores[storeID].StoreName + " reopen.");
        }
        public void DeleteStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores.TryRemove(storeID, out var store);
            Logger.Instance.Info("store " + store.StoreName + " deleted.");
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
        public double PurchaseCart(Dictionary<Guid, Dictionary<Guid, int>> items, ref List<ItemForOrder> itemForOrders)
        {
            IsTsInitialized();
            double sum = 0; 
            Dictionary<Guid, Dictionary<Guid, int>> storeUpdated = new Dictionary<Guid, Dictionary<Guid, int>>(); //store that the inventory already update
            try
            {
                foreach (Guid storeID in items.Keys)
                {
                    IsStoreExist(storeID); // not possible but still...
                    if (!stores[storeID].Active)
                        throw new Exception($"The store: {storeID} not active");
                     sum += stores[storeID].PurchaseCart(items[storeID], ref itemForOrders);
                    storeUpdated.Add(storeID, items[storeID]);
                }
                return sum;
            }
            catch (Exception e)
            {
                AddItemToStores(storeUpdated);
                throw;
            }
        }

        public List<Store> GetAllStoreInfo()
        {
            IsTsInitialized();
            return stores.Values.ToList();
        }
        public void AddItemToStores(Dictionary<Guid, Dictionary<Guid, int>> items)
        {
            foreach (Guid storeID in items.Keys)
            {
                foreach (Guid itemID in items[storeID].Keys)
                {
                    stores[storeID].EditItemQuantity(itemID, items[storeID][itemID]);
                }
            }
        }

        public Guid AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            return stores[storeID].AddItem(itemName, itemCategory, itemPrice, quantity);
        }

        public void RemoveItemFromStore(Guid storeID, Guid itemId)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].RemoveItem(itemId);
        }
        
        public void EditItemName(Guid storeID, Guid itemID, string name)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemName(itemID, name);
        }
        
        public void EditItemCategory(Guid storeID, Guid itemID, string category)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemCategory(itemID, category);
        }
        public void EditItemPrice(Guid storeID, Guid itemID, int price)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemPrice(itemID, price);
        }
        public void EditItemQuantity(Guid storeID, Guid itemID, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemQuantity(itemID, quantity);
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
            foreach (var item in from order in _orders.GetOrdersByUserId(userID) from item in order.ListItems where item.ItemID == itemID select item)
            {
                foundUserOrder = true;
            }
            if (!foundUserOrder)
                throw new Exception("user with id:" + userID + "tried writing review to item: " + itemID + " which he did not purchase before");

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
