using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer;
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

        public ResponseT<ShoppingCart> GetDetailsOnCart(Guid userID)
        {
            try
            {
                return new ResponseT<ShoppingCart>(userFacade.GetDetailsOnCart(userID));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(GetDetailsOnCart)+": "+ex.Message);
                return new ResponseT<ShoppingCart>(ex.Message);
            }
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(AddItemToCart)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(RemoveItemFromCart)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(EditItemFromCart)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response PurchaseCart(Guid userID, string paymentDetails, string usersDetail)
        {
            Dictionary<Guid, Dictionary<Guid, int>> cart = new Dictionary<Guid, Dictionary<Guid, int>>();
            try
            {
                // send list of objects as ref 
                List<ItemForOrder> itemForOrders = new List<ItemForOrder>();
                //get the user cart
                ShoppingCart shoppingCart = userFacade.GetDetailsOnCart(userID);
                if (shoppingCart.Baskets.Count == 0)
                    throw new Exception("Cart can't be empty");
                // cast from shopping cart to dictionary before sending to store component.
                foreach (ShoppingBasket basket in shoppingCart.Baskets) 
                    cart.Add(basket.StoreID, basket.ItemsInBasket);
                // try to purchase the items. (the function update the quantity in the inventory in this function)
                double amount = storeFacade.PurchaseCart(cart, ref itemForOrders);
                if (!userFacade.PlacePayment(amount, paymentDetails))
                {
                    storeFacade.AddItemToStores(cart); // because we update the inventory we need to return them to inventory.
                    throw new Exception("The payment not work!");
                }
                if (!userFacade.PlaceSupply(shoppingCart.ToString(), usersDetail))
                {
                    storeFacade.AddItemToStores(cart); // because we update the inventory we need to return them to inventory.
                    userFacade.CancelPayment(amount, paymentDetails); // because we need to refund the user
                    throw new Exception("The supply not work!");
                }
                Orders.Instance.AddOrder(userID, itemForOrders);
                userFacade.PurchaseCart(userID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public ResponseT<Guid> OpenNewStore(Guid userID, string storeName)
        {
            try
            {
                Guid storeID = storeFacade.OpenNewStore(storeName);
                userFacade.OpenNewStore(userID,storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(OpenNewStore)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public Response CloseStore(Guid userID ,Guid storeID)
        {
            try
            {
                userFacade.CloseStore(userID, storeID);
                storeFacade.CloseStore(storeID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(CloseStore)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response DeleteStore(Guid userID ,Guid storeID)
        {
            try
            {
                storeFacade.DeleteStore(storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(DeleteStore)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }
        public Response ReopenStore(Guid userID ,Guid storeID)
        {
            try
            {
                storeFacade.ReopenStore(storeID);
                return new ResponseT<Guid>(storeID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(ReopenStore)+": "+ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(AddItemToStore)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(RemoveItemFromStore)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public Response WriteItemReview(Guid userID, Guid storeID, Guid itemID, string reviewText)
        {
            try
            {
                userFacade.isLoggedIn(userID);
                storeFacade.WriteItemReview(userID, storeID, itemID, reviewText);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(WriteItemReview)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public ResponseT<ConcurrentDictionary<Guid, List<string>>> GetItemReviews(Guid storeID, Guid itemID)
        {
            ConcurrentDictionary<Guid, List<string>> reviews = storeFacade.GetItemReviews(storeID, itemID);
            return new ResponseT<ConcurrentDictionary<Guid, List<string>>>(reviews);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(EditItemPrice)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(EditItemCategory)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(EditItemName)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(EditItemQuantity)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public ResponseT<List<Store>> GetAllStoreInfo()
        {
            List<Store> stores = storeFacade.GetAllStoreInfo();
            return new ResponseT<List<Store>>(stores);
        }
        
        public ResponseT<List<Item>> GetItemsByName(Guid userID, string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            try
            {
                List<Item> items = storeFacade.GetItemsByName(itemName, minPrice, maxPrice, ratingItem, category, ratingStore);
                return new ResponseT<List<Item>>(items);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(GetItemsByName)+": "+ex.Message);
                return new ResponseT<List<Item>>(ex.Message);
            }
        }

        public ResponseT<List<Item>> GetItemsByCategory(Guid userID, string category, int minPrice, int maxPrice, int ratingItem, int ratingStore)
        {
            try
            {
                List<Item> items = storeFacade.GetItemsByCategory(category, minPrice, maxPrice, ratingItem, ratingStore);
                return new ResponseT<List<Item>>(items);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(GetItemsByCategory)+": "+ex.Message);
                return new ResponseT<List<Item>>(ex.Message);
            }
        }
        
        public ResponseT<List<Item>> GetItemsByKeysWord(Guid userID, string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            try
            {
                List<Item> items = storeFacade.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
                return new ResponseT<List<Item>>(items);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(GetItemsByKeysWord)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(GetStorePurchases)+": "+ex.Message);
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
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(GetAllStorePurchases)+": "+ex.Message);
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

        public ResponseT<Store> GetStore(Guid storeID)
        {
            try
            {
                Store store = storeFacade.GetStore(storeID);
                return new ResponseT<Store>(store);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetStore) + ": " + ex.Message);
                return new ResponseT<Store>(ex.Message);
            }
        }

        public void SetTSOrders(IOrders orders)
        {
            storeFacade.SetTSOrders(orders);
        }

        public ResponseT<Item> GetItemByID(Guid storeID, Guid itemID)
        {
            try
            {
                Item item = storeFacade.GetItemByID(storeID, itemID);
                return new ResponseT<Item>(item);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetStore) + ": " + ex.Message);
                return new ResponseT<Item>(ex.Message);
            }
        }
    }
}