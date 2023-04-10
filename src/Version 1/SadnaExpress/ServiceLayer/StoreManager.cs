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


        public Response PurchaseCart(int id, string paymentDetails)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> OpenNewStore(int id, string storeName)
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
        public Response CloseStore(int id ,Guid storeID)
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
        public Response DeleteStore(int id ,Guid storeID)
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
        public Response ReopenStore(int id ,Guid storeID)
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
        public Response GetPurchasesInfo(int id ,Guid storeID)
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


        public Response WriteReview(int id, int itemID, string review)
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
        
        public ResponseT<List<S_Store>> GetAllStoreInfo(int id)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByName(int id, string itemName)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByCategory(int id, string category)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByKeysWord(int id, string keyWords)
        {
            throw new System.NotImplementedException();
        }
        

        public Response GetPurchasesInfo(int id, int storeID)
        {
            throw new System.NotImplementedException();
        }
        
        public ResponseT<List<S_Item>> GetItemsByPrices(int id, int minPrice, int maxPrice)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByItemRating(int id, int rating)
        {
            throw new System.NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByStoreRating(int id, int rating)
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