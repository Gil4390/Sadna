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

        public ResponseT<Dictionary<Guid, List<Guid>>> GetDetailsOnCart(Guid userID)
        {
            throw new NotImplementedException();
        }
        
        public Response AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            try
            {
                storeFacade.AddItemToCart(storeID, itemID, itemAmount);
                userFacade.AddItemToCart(userID, storeID, itemID, itemAmount);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID)
        {
            try
            {
                userFacade.RemoveItemFromCart(userID, storeID, itemID);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }

        public Response EditItemFromCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            try
            {
                userFacade.EditItemFromCart(userID, storeID, itemID, itemAmount);
                return new Response();

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
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

        public Response WriteReview(Guid id, Guid itemID, string review)
        {
            throw new NotImplementedException();
        }

        public Response CloseStore(Guid id ,Guid storeID)
        {
            try
            {
                userFacade.CloseStore(id, storeID);
                storeFacade.CloseStore(storeID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
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
        public Response PurchaseItems(int id, string paymentDetails)
        {
            throw new System.NotImplementedException();
        }
        public ResponseT<Guid> AddItemToStore(Guid userID, Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            try
            {
                userFacade.AddItemToStore(userID, storeID);
                return new ResponseT<Guid>(storeFacade.AddItemToStore(storeID, itemName, itemCategory, itemPrice, quantity));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public Response RemoveItemFromStore(Guid userID, Guid storeID, Guid itemID)
        {
            try
            {
                userFacade.RemoveItemFromStore(userID, storeID);
                storeFacade.RemoveItemFromStore(storeID, itemID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response WriteItemReview(Guid userID, Guid storeID, Guid itemID, string reviewText)
        {
            storeFacade.WriteItemReview(userID, storeID, itemID, reviewText);
            return new Response();
        }
        public Response EditItemPrice(Guid userID, Guid storeID, Guid itemID, int price)
        {
            try
            {
                userFacade.EditItem(userID, storeID);
                storeFacade.EditItemPrice(storeID, itemID, price);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response EditItemCategory(Guid userID, Guid storeID, Guid itemID, string category)
        {
            try
            {
                userFacade.EditItem(userID, storeID);
                storeFacade.EditItemCategory(storeID, itemID, category);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response EditItemName(Guid userID, Guid storeID, Guid itemID, string name)
        {
            try
            {
                userFacade.EditItem(userID, storeID);
                storeFacade.EditItemName(storeID, itemID, name);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response EditItemQuantity(Guid userID, Guid storeID, Guid itemID, int quantity)
        {
            try
            {
                userFacade.EditItem(userID, storeID);
                storeFacade.EditItemQuantity(storeID, itemID, quantity);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response(ex.Message);
            }
        }
        public ResponseT<List<Store>> GetAllStoreInfo(Guid id)
        {
            throw new System.NotImplementedException();
        }
        
        public ResponseT<List<Item>> GetItemsByName(Guid id, string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            try
            {
                List<Item> items = storeFacade.GetItemsByName(itemName, minPrice, maxPrice, ratingItem, category, ratingStore);
                return new ResponseT<List<Item>>(items);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<Item>>(ex.Message);
            }
        }

        public ResponseT<List<Item>> GetItemsByCategory(Guid id, string category, int minPrice, int maxPrice, int ratingItem, int ratingStore)
        {
            try
            {
                List<Item> items = storeFacade.GetItemsByCategory(category, minPrice, maxPrice, ratingItem, ratingStore);
                return new ResponseT<List<Item>>(items);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<Item>>(ex.Message);
            }
        }
        
        public ResponseT<List<Item>> GetItemsByKeysWord(Guid id, string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            try
            {
                List<Item> items = storeFacade.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
                return new ResponseT<List<Item>>(items);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<Item>>(ex.Message);
            }
        }
        public ResponseT<List<Order>> GetStorePurchases(Guid userID, Guid storeID)
        {
            try
            {
                userFacade.GetStorePurchases(userID, storeID);
                List<Order> purchases = storeFacade.GetStorePurchases(storeID);
                return new ResponseT<List<Order>>(purchases);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<Order>>(ex.Message);
            }
        }

        public ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID)
        {
            try
            {
                userFacade.GetAllStorePurchases(userID);
                Dictionary<Guid, List<Order>> purchases = storeFacade.GetAllStorePurchases();
                return new ResponseT<Dictionary<Guid, List<Order>>>(purchases);
            }
            catch(Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Dictionary<Guid, List<Order>>>(ex.Message);

            }
        }
        public void CleanUp()
        {
            userFacade.CleanUp();
        }
        public ConcurrentDictionary<Guid, Store> GetStores()
        {
            return storeFacade.GetStores();
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            storeFacade.SetIsSystemInitialize(isInitialize);
        }
    }
}