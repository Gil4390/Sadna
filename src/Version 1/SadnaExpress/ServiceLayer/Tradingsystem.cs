using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;


namespace SadnaExpress.ServiceLayer
{
    public class TradingSystem : ITradingSystem
    {
        private IStoreManager storeManager;
        private IUserManager userManager;

        private bool testMode=false;
        public bool TestMode
        {
            get
            {
                return testMode;
            }
            set
            {
                if (testMode != value)
                    Logger.Instance.SwitchOutputFile();
                testMode = value;
            }
        }

        // lock object for the instance
        private static readonly object lockInstance = new object();
        private static TradingSystem instance = null;

        public TradingSystem(IPaymentService paymentService = null, ISupplierService supplierService=null)
        {
            IUserFacade userFacade = new UserFacade(paymentService, supplierService);
            IStoreFacade storeFacade = new StoreFacade();
            storeManager = new StoreManager(userFacade, storeFacade);
            userManager = new UserManager(userFacade);
        }
        
        public TradingSystem(IUserFacade userFacade, IStoreFacade storeFacade)
        {
            storeManager = new StoreManager(userFacade, storeFacade);
            userManager = new UserManager(userFacade);
        }

        public static TradingSystem Instance
        {
            get
            {
                lock (lockInstance)
                {
                    if (instance == null)
                    {
                        instance = new TradingSystem();
                    }
                    return instance;
                }
            }
        }
        
        public void SetIsSystemInitialize(bool isInitialize)
        {
            userManager.SetIsSystemInitialize(isInitialize);
            storeManager.SetIsSystemInitialize(isInitialize);
        }

        public int GetMaximumWaitServiceTime()
        {
            return 10000;
        }
        public ResponseT<Guid> Enter()
        {
            return userManager.Enter();
        }
        public Response Exit(Guid userID)
        {
            return userManager.Exit(userID);
        }
        public Response Register(Guid userID, string email, string firstName, string lastName, string password)
        {
            return userManager.Register(userID, email, firstName, lastName, password);
        }
        public ResponseT<Guid> Login(Guid userID, string email, string password)
        {
            return userManager.Login(userID, email, password);
        }
        public ResponseT<Guid> Logout(Guid userID)
        {
            return userManager.Logout(userID);

        }
        public ResponseT<bool> InitializeTradingSystem(Guid userID)
        {
            try
            {
                Logger.Instance.Info("User id: " + userID + " requested to initialize trading system");
                ResponseT<bool> responseT= userManager.InitializeTradingSystem(userID);
                if (responseT.Value)
                    storeManager.SetIsSystemInitialize(true);
                return responseT;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<List<Store>> GetAllStoreInfo()
        {
            try
            {
                Logger.Instance.Info("GetAllStoreInfo");
                return storeManager.GetAllStoreInfo();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<Store>>(ex.Message);
            }
            
        }
        public ResponseT<Guid> OpenNewStore(Guid userID, string storeName)
        {
            return storeManager.OpenNewStore(userID, storeName);
        }
        public ResponseT<List<Item>> GetItemsByName(Guid userID, string itemName, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            return storeManager.GetItemsByName(userID, itemName, minPrice, maxPrice, ratingItem, category, ratingStore);
        }
        public ResponseT<List<Item>> GetItemsByCategory(Guid userID, string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1)
        {
            return storeManager.GetItemsByCategory(userID, category, minPrice, maxPrice, ratingItem, ratingStore);
        }

        public ResponseT<List<Item>> GetItemsByKeysWord(Guid userID, string keyWords, int minPrice = 0,
            int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            return storeManager.GetItemsByKeysWord(userID, keyWords, minPrice, maxPrice, ratingItem, category,
                ratingStore);
        }

        public Response AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            return storeManager.AddItemToCart(userID, storeID, itemID, itemAmount);
        }

        public Response RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID)
        {
            return storeManager.RemoveItemFromCart(userID, storeID, itemID);
        }

        public Response EditItemFromCart(Guid userID, Guid storeID, Guid itemID, int itemAmount)
        {
            return storeManager.EditItemFromCart(userID, storeID, itemID, itemAmount);
        }

        public ResponseT<ShoppingCart> GetDetailsOnCart(Guid userID)
        {
            return storeManager.GetDetailsOnCart(userID);
        }
        public Response PurchaseCart(Guid userID, string paymentDetails, string usersDetail)
        {
            return storeManager.PurchaseCart(userID, paymentDetails, usersDetail);
        }
        public Response WriteItemReview(Guid userID, Guid storeID, Guid itemID, string review)
        {
            try
            {
                Logger.Instance.Info("User id: " + userID + " WriteReview to itemID: " + itemID);
                return storeManager.WriteItemReview(userID, storeID, itemID, review);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("error adding review to item");
                return new Response(ex.Message);
            }
        }

        public ResponseT<ConcurrentDictionary<Guid, List<string>>> GetItemReviews(Guid storeID, Guid itemID)
        {
            try
            {
                Logger.Instance.Info("getItemReviews on itemID: " + itemID);
                return storeManager.GetItemReviews(storeID, itemID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("error fetching reviews of item");
                return new ResponseT<ConcurrentDictionary<Guid, List<string>>>(ex.Message);
            }
        }

        public Response RateItem(Guid userID, int itemID, int score)
        {
            throw new NotImplementedException();
        }

        public Response WriteMessageToStore(Guid userID, Guid storeID, string message)
        {
            throw new NotImplementedException();
        }

        public Response ComplainToAdmin(Guid userID, string message)
        {
            throw new NotImplementedException();
        }

        public Response GetPurchasesInfoUser(Guid userID)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> AddItemToStore(Guid userID, Guid storeID,  string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return storeManager.AddItemToStore(userID, storeID, itemName, itemCategory, itemPrice, quantity);
        }

        public Response RemoveItemFromStore(Guid userID, Guid storeID, Guid itemID)
        {
            return storeManager.RemoveItemFromStore(userID, storeID, itemID);
        }
        public Response EditItemCategory(Guid userID, Guid storeID, Guid itemID, string category)
        {
            return storeManager.EditItemCategory(userID, storeID, itemID, category);
        }

        public Response EditItemPrice(Guid userID, Guid storeID,  Guid itemID, int price)
        {
            return storeManager.EditItemPrice(userID, storeID, itemID, price);
        }
        public Response EditItemName(Guid userID, Guid storeID,  Guid itemID, string name)
        {
            return storeManager.EditItemName(userID, storeID, itemID, name);
        }
        public Response EditItemQuantity(Guid userID, Guid storeID, Guid itemID, int quantity)
        {
            // if you want remove put -i and to add +i
            return storeManager.EditItemQuantity(userID, storeID, itemID, quantity);
        }
        public Response AppointStoreOwner(Guid userID, Guid storeID, string userEmail)
        {
            return userManager.AppointStoreOwner(userID, storeID, userEmail);
        }

        public Response AppointStoreManager(Guid userID, Guid storeID, string userEmail)
        {
            return userManager.AppointStoreManager(userID, storeID, userEmail);
        }

        public Response AddStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission)
        {
            return userManager.AddStoreManagerPermissions(userID, storeID, userEmail, permission);
        }

        public Response RemoveStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission)
        {
            return userManager.RemoveStoreManagerPermissions(userID, storeID, userEmail, permission);
        }

        public Response RemoveStoreManager(Guid userID1, Guid storeID, Guid userID2)
        {
            throw new NotImplementedException();
        }

        public Response RemoveStoreOwner(Guid userID1, Guid storeID, Guid userID2)
        {
            throw new NotImplementedException();
        }

        public Response CloseStore(Guid userID, Guid storeID)
        {
            return storeManager.CloseStore(userID,storeID);
        }

        public Response ReopenStore(Guid userID, Guid storeID)
        {
            return storeManager.ReopenStore(userID,storeID);
        }

        public ResponseT<List<PromotedMember>> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            return userManager.GetEmployeeInfoInStore(userID, storeID);
        }

        public ResponseT<List<Order>> GetStorePurchases(Guid userID, Guid storeID)
        {
            return storeManager.GetStorePurchases(userID, storeID);
        }

        public ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID)
        {
            return storeManager.GetAllStorePurchases(userID);
        }
        
        public Response DeleteStore(Guid userID, Guid storeID)
        {
            return storeManager.DeleteStore(userID, storeID);
        }

        public Response DeleteMember(Guid userID1, Guid userID2)
        {
            throw new NotImplementedException();
        }
        public ResponseT<Guid> UpdateFirst(Guid userID, string newFirst)
        {
            return userManager.UpdateFirst(userID, newFirst);
        }

        public ResponseT<Guid> UpdateLast(Guid userID, string newLast)
        {
            return userManager.UpdateLast(userID, newLast);
        }

        public ResponseT<Guid> UpdatePassword(Guid userID, string newPassword)
        {
            return userManager.UpdatePassword(userID, newPassword);
        }

        public ResponseT<Guid> SetSecurityQA(Guid userID,string q, string a)
        {
            return userManager.SetSecurityQA(userID,q,a);
        }

        public void CleanUp() // for the tests
        {
            storeManager.CleanUp();
            userManager.CleanUp();
        }
        
        public ConcurrentDictionary<Guid, User> GetCurrent_Users()
        {
            return userManager.GetCurrent_Users();
        }
        public ConcurrentDictionary<Guid, Member> GetMembers()
        {
            return userManager.GetMembers();
        }
        public ConcurrentDictionary<Guid , Store> GetStores()
        {
            return storeManager.GetStores();
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            userManager.SetPaymentService(paymentService);
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            userManager.SetSupplierService(supplierService);
        }


        //public Response PurchaseCart(Guid userID, string paymentDetails)
        //{
        //    bool purchaseSuccess = false;
        //    // saving items for userid cart items on purchase cart action
        //    lock (stockChange)
        //    {
        //        if (savedItemsRestore.ContainsKey(userID))
        //            savedItemsRestore[userID] = new Dictionary<Guid, List<Pair<int, int>>>();
        //        else
        //            savedItemsRestore.Add(userID, new Dictionary<Guid, List<Pair<int, int>>>());
        //        // get user shopping cart inventory and stock and reduce all stock and save the reduced value
        //        ShoppingCart cart = userManager.GetShoppingCartById(userID).Value;
        //        foreach (ShoppingBasket basket in cart.GetShoppingBaskets())
        //        {
        //            Store store = storeManager.GetStoreById(basket.GetStoreId());
        //            Dictionary<int, int> itemsInBasket = basket.GetItemsInBasket();
        //            foreach (int itemId in itemsInBasket.Keys)
        //            {
        //                store.RemoveQuantity(itemId, itemsInBasket[itemId]);
        //                if (savedItemsRestore[userID].ContainsKey(basket.GetStoreId()))
        //                {
        //                    savedItemsRestore[userID][basket.GetStoreId()].Add(new Pair<int, int>(itemId, basket.GetItemStock(itemId)));
        //                }
        //                else
        //                {
        //                    savedItemsRestore[userID].Add(basket.GetStoreId(), new List<Pair<int, int>>());
        //                    savedItemsRestore[userID][basket.GetStoreId()].Add(new Pair<int, int>(itemId, basket.GetItemStock(itemId)));
        //                }
        //            }
        //        }
        //        Logger.Instance.Info("PurhcaseCart, saving store items for userid: " + userID);
        //    }
            /////////////////////////// end of saving items implementation

            // todo: need to implement purchase cart action and
            // update bool purchaseSuccess to the purchase result


        //    // in case of purchase failure restore store items quantity
        //    if (purchaseSuccess.Equals(false))
        //    {
        //        lock (stockChange)
        //        {
        //            foreach (Guid storeId in savedItemsRestore[userID].Keys)
        //            {
        //                foreach (Pair<int, int> storeItem in savedItemsRestore[userID][storeId])
        //                {
        //                    storeManager.GetStoreById(storeId).AddQuantity(storeItem.First, storeItem.Second);
        //                }
        //            }
        //            Logger.Instance.Info("Purchase Failed restored saved store items for userid: "+userID);
        //            savedItemsRestore.Remove(userID);
        //        }
        //    }
        //    return new Response();
        //}

        public ResponseT<User> GetUser(Guid userID)
        {
            return userManager.GetUser(userID);
        }

        public ResponseT<Member> GetMember(Guid userID)
        {
            return userManager.GetMember(userID);
        }

        public ResponseT<ShoppingCart> GetUserShoppingCart(Guid userID)
        {
            return userManager.GetUserShoppingCart(userID);
        }

        public ResponseT<Store> GetStore(Guid storeID)
        {
            return storeManager.GetStore(storeID);
        }

        public void SetTSOrders(IOrders orders)
        {
            storeManager.SetTSOrders(orders);
        }

        public ResponseT<Item> GetItemByID(Guid storeID, Guid itemID)
        {
            return storeManager.GetItemByID(storeID, itemID);
        }
    }
}