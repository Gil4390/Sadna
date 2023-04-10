using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        private ConcurrentDictionary<Guid, Store> stores;
        private ConcurrentDictionary<Guid, LinkedList<Order>> storeOrders;
        private ISupplierService supplierService;
        public ISupplierService SupplierService { get => supplierService; set => supplierService = value; }
        private const int MaxSupplyServiceWaitTime = 10000; //10 seconds is 10,000 mili seconds

        public StoreFacade(ISupplierService supplierService=null)
        {
            stores = new ConcurrentDictionary<Guid, Store>();
            storeOrders = new ConcurrentDictionary<Guid, LinkedList<Order>>();
            this.supplierService = supplierService;
        }
        
        public Store GetStoreByID(Guid id)
        {
            foreach (Store store in stores.Values) {
                if (store.StoreId.Equals(id))
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
            Logger.Instance.Info("store " + store.getName() + " closed.");
        }
        public void ReopenStore(Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            //
            store.Active = true;
            Logger.Instance.Info("store " + store.getName() + " reopen.");
        }
        public void DeleteStore(Guid storeId)
        {
            Store store = GetStoreByID(storeId);
            if (store == null)
                Logger.Instance.Error("there is no store with this name");
            stores.TryRemove(store.StoreID, out store);
            Logger.Instance.Info("store " + store.getName() + " deleted.");
        }
        
        public LinkedList<Order> GetStorePurchases(Guid storeId)
        {
            if (!storeOrders.ContainsKey(storeId))
                throw new Exception("Store with this id does not exist");
            Logger.Instance.Info("store " + GetStoreByID(storeId).getName() + " got his purchases info.");
            return storeOrders[storeId];
        }
        public void PurchaseItems(string storeName, List<string> itemsName)
        {   
            throw new System.NotImplementedException();

        }

        public void GetAllStoreInfo(string storeName)
        {
            throw new System.NotImplementedException();

        }

        public void AddItemToStore(string storeName, string itemName, string category, int price)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItemFromStore(string storeName, string itemName, string category, int price)
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

        public void WriteReview(string storeName, string itemName, int rating)
        {
            throw new System.NotImplementedException();
        }

        public void CleanUp()
        {
           storeOrders.Clear();
           stores.Clear();
           supplierService = null;
        }
        public ConcurrentDictionary<Guid, Store> GetStores()
        {
            return stores;
        }
        public void SetSupplierService(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public bool PlaceSupply(string orderDetails, string userDetails)
        {
            try
            {
                Logger.Instance.Info(nameof(supplierService)+": user: "+userDetails+" request to preform a supply for order: "+orderDetails);

                var task = Task.Run(() =>
                {
                    return supplierService.ShipOrder(orderDetails, userDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(MaxSupplyServiceWaitTime));

                if (isCompletedSuccessfully)
                {
                    return true;
                }
                else
                {
                    throw new TimeoutException("Supply external service action has taken longer than the maximum time allowed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return false;
            }
        }
    }
}