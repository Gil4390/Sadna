using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;

namespace SadnaExpress.ServiceLayer
{
    public class StoreManager: IStoreManager
    {
        private IStoreFacade storeFacade;
        private IUserFacade userFacade;
       
        public StoreManager(IUserFacade userFacade, IStoreFacade storeFacade)
        {
            this.userFacade = userFacade;
            this.storeFacade = storeFacade;
            
        }

        public Response PurchaseCart(Guid id, string paymentDetails)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> OpenNewStore(Guid id, string storeName)
        {
            try
            {
                Guid storeID = storeFacade.OpenNewStore(storeName);
                userFacade.OpenNewStore(id,storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public Response CloseStore(Guid id ,Guid storeID)
        {
            try
            {
                storeFacade.CloseStore(storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public Response DeleteStore(Guid id ,Guid storeID)
        {
            try
            {
                storeFacade.DeleteStore(storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public Response ReopenStore(Guid id ,Guid storeID)
        {
            try
            {
                storeFacade.ReopenStore(storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public Response GetPurchasesInfo(Guid id, Guid storeID)
        {
            try
            {
                storeFacade.GetStorePurchases(storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public Response AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            try
            {
                storeFacade.AddItemToStore(storeID, itemName, itemCategory, itemPrice, quantity);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response PurchaseItems(int id, string paymentDetails)
        {
            throw new System.NotImplementedException();
        }


        public Response WriteReview(Guid id, int itemID, string review)
        {
            throw new System.NotImplementedException();
        }

    

        public Response RemoveItemFromStore(Guid storeID, int itemID)
        {
            try
            {
                storeFacade.RemoveItemFromStore(storeID, itemID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response EditItemPrice(string storeID, string itemName, int price)
        {
            throw new System.NotImplementedException();
        }
        public Response EditItemCategory(string storeID, string itemName, string category)
        {
            throw new System.NotImplementedException();
        }
        
        public ResponseT<List<S_Store>> GetAllStoreInfo(Guid id)
        {
            throw new System.NotImplementedException();
        }
        
        public ResponseT<List<S_Item>> GetItemsByName(Guid id, string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            try
            {
                // change to s_item
                List<Item> items = storeFacade.GetItemsByName(itemName, minPrice, maxPrice, ratingItem, category, ratingStore);
                return new ResponseT<List<S_Item>>();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<S_Item>>(ex.Message);
            }
        }

        public ResponseT<List<S_Item>> GetItemsByCategory(Guid id, string category, int minPrice, int maxPrice, int ratingItem, int ratingStore)
        {
            try
            {
                // change to s_item
                List<Item> items = storeFacade.GetItemsByCategory(category, minPrice, maxPrice, ratingItem, ratingStore);
                return new ResponseT<List<S_Item>>();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<S_Item>>(ex.Message);
            }
        }
        
        public ResponseT<List<S_Item>> GetItemsByKeysWord(Guid id, string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            try
            {
                // change to s_item
                List<Item> items = storeFacade.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
                return new ResponseT<List<S_Item>>();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<S_Item>>(ex.Message);
            }
        }
        
        public Response GetPurchasesInfo(Guid id, int storeID)
        {
            throw new System.NotImplementedException();
        }
        
        public void CleanUp()
        {
            userFacade.CleanUp();
        }
        public ConcurrentDictionary<Guid, Store> GetStores()
        {
            return storeFacade.GetStores();
        }


        public Store GetStoreById(Guid storeId)
        {
            return storeFacade.GetStoreById(storeId);
        }

    }
}