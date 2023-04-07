using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;

namespace SadnaExpress.ServiceLayer
{
    public class StoreManager: IStoreManager
    {
        private IStoreFacade storeFacade;
        private IUserFacade userFacade;
        
        public StoreManager()
        {
            userFacade = new UserFacade();
            storeFacade = new StoreFacade();
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

        public Response PurchaseCart(int id, string paymentDetails)
        {
            throw new System.NotImplementedException();
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

        public Response WriteReview(int id, int itemID, string review)
        {
            throw new System.NotImplementedException();
        }

        public Response AddItemToStore(int id, int storeID, string itemName, string itemCategory, float itemPrice)
        {
            throw new System.NotImplementedException();
        }

        public Response RemoveItemFromStore(int id, int itemID)
        {
            throw new System.NotImplementedException();
        }

        public Response EditItemCategory(string storeID, string itemName, string category)
        {
            throw new System.NotImplementedException();
        }

        public Response EditItemPrice(string storeID, string itemName, int price)
        {
            throw new System.NotImplementedException();
        }

        public Response CloseStore(int id, int storeID)
        {
            throw new System.NotImplementedException();
        }

        public Response GetPurchasesInfo(int id, int storeID)
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
    }
}