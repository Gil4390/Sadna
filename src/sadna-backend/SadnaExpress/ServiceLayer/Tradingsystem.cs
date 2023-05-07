using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ExternalServices;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.ServiceLayer.SModels;
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
            //if services are null initialized with default services
            if (paymentService == null)
                paymentService = new PaymentService();
            if (supplierService == null)
                supplierService = new SupplierService();

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
            ResponseT<Guid> responseT;
            try
            {
                
                responseT = storeManager.OpenNewStore(userID, storeName);
                GetMember(userID).Value.Update(" " + userID + "open new store", userID);
                return responseT;

            } 
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
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

        public ResponseT<List<SItem>> GetItemsForClient(Guid userID, string keyWords, int minPrice = 0,
            int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            ResponseT<List<Item>> res= storeManager.GetItemsByKeysWord(userID, keyWords, minPrice, maxPrice, ratingItem, category,
                ratingStore);

            List<SItem> items = new List<SItem>();
            foreach (Item item in res.Value)
            {
                Guid itemStoreid= storeManager.GetItemStoreId(item.ItemID);
                bool inStock = storeManager.GetStore(itemStoreid).Value.GetItemByQuantity(item.ItemID) > 0;
                int countInCart = storeManager.GetItemQuantityInCart(userID,itemStoreid, item.ItemID).Value;
                items.Add(new SItem(item,itemStoreid, inStock, countInCart));
            }

            return new ResponseT<List<SItem>>(items);
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

        public ResponseT<List<SItem>> GetCartItems(Guid userID)
        {
            return storeManager.GetCartItems(userID);
        }

        public ResponseT<List<ItemForOrder>> PurchaseCart(Guid userID, string paymentDetails, string usersDetail)
        {
            ResponseT<List<ItemForOrder>>  response = storeManager.PurchaseCart(userID, paymentDetails, usersDetail);
            return response;
        }
        
        public Response WriteItemReview(Guid userID, Guid itemID, string review)
        {
            try
            {
                Logger.Instance.Info("User id: " + userID + " WriteReview to itemID: " + itemID);
                return storeManager.WriteItemReview(userID, itemID, review);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("error adding review to item");
                return new Response(ex.Message);
            }
        }

        public ResponseT<List<Review>> GetItemReviews(Guid storeID, Guid itemID)
        {
            try
            {
                Logger.Instance.Info("getItemReviews on itemID: " + itemID);
                return storeManager.GetItemReviews(storeID, itemID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("error fetching reviews of item");
                return new ResponseT<List<Review>>(ex.Message);
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

        public ResponseT<Dictionary<Guid, List<Order>>> GetPurchasesInfoUser(Guid userID)
        {
            Dictionary<Guid, List<Order>> orders = Orders.Instance.GetUserOrders();
            
            return new ResponseT<Dictionary<Guid, List<Order>>>(orders);
        }
        public ResponseT<List<ItemForOrder>> GetPurchasesInfoUserOnlu(Guid userID)
        {
            List<ItemForOrder> list = new List<ItemForOrder>();
            if (Orders.Instance.GetUserOrders().ContainsKey(userID))
            {
                foreach (Order order in Orders.Instance.GetUserOrders()[userID])
                {
                    list.AddRange(order.ListItems);
                }
            }
            return new ResponseT<List<ItemForOrder>>(list);
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

        public Response EditItem(Guid userID, Guid storeID,  Guid itemID,  string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return storeManager.EditItem(userID, storeID,itemID, itemName, itemCategory, itemPrice, quantity);
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

        public Response RemoveStoreOwner(Guid userID, Guid storeID, string userEmail)
        {
            return userManager.RemoveStoreOwner(userID, storeID, userEmail);
        }

        public Response CloseStore(Guid userID, Guid storeID)
        {
            Response response =  storeManager.CloseStore(userID,storeID);
            return response;
        }

        public Response ReopenStore(Guid userID, Guid storeID)
        {
            throw new NotImplementedException();
            //notificationSystem.NotifyObservers(storeID,"reopen store",userID);
            //return storeManager.ReopenStore(userID,storeID);
        }

        public ResponseT<List<SMemberForStore>> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            return userManager.GetEmployeeInfoInStore(userID, storeID);
        }

        public Response RemoveUserMembership(Guid userID, string email)
        {
            return userManager.RemoveUserMembership(userID, email);
        }

        public ResponseT<List<ItemForOrder>> GetStorePurchases(Guid userID, Guid storeID)
        {
            List<ItemForOrder> list = new List<ItemForOrder>();
            if (Orders.Instance.GetStoreOrders().ContainsKey(storeID))
            {
                foreach (Order order in Orders.Instance.GetStoreOrders()[storeID])
                {
                    list.AddRange(order.ListItems);
                }
            }
            return new ResponseT<List<ItemForOrder>>(list);
        }
        public Response RemovePermission(Guid userID, Guid storeID, string userEmail, string permission)
        {
            return userManager.RemovePermission(userID, storeID, userEmail, permission);
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
        public ResponseT<List<SMember>> GetMembers(Guid userID)
        {
            return userManager.GetMembers(userID);
        }
        public ResponseT<ConcurrentDictionary<Guid , Store>> GetStores()
        {
            return new ResponseT<ConcurrentDictionary<Guid, Store>>(storeManager.GetStores());
        }

        public ResponseT<List<Member>> GetStoreOwners()
        {
            ConcurrentDictionary<Guid, Store> stores = storeManager.GetStores();
            return userManager.getAllStoreOwners(stores);
        }

        public ResponseT<List<Member>> GetStoreOwnerOfStores(List<Guid> stores)
        {
            return userManager.GetStoreOwnerOfStores(stores);
        }

        public ResponseT<List<Item>> GetItemsInStore(Guid userID, Guid storeId)
        {
            return storeManager.GetItemsInStore(userID,storeId);
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            userManager.SetPaymentService(paymentService);
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            userManager.SetSupplierService(supplierService);
        }

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

        public ResponseT<List<Notification>> GetNotifications(Guid userID)
        {
            return userManager.GetNotifications(userID);
        }

        // this functions needs to notify to offline members their notifications.
        public void getNotificationsForOfflineMembers()
        {
            throw new NotImplementedException();
        }
        public bool IsSystemInitialize()
        {
            return userManager.IsSystemInitialize();
        }

        public ResponseT<PurchaseCondition[] > GetAllConditions(Guid store)
        {
            return storeManager.GetAllConditions(store);
        }

        public ResponseT<Condition> GetCondition<T, M>(Guid store , T entity, string type, double value,DateTime dt=default, M entityRes = default, string typeRes = default,
            double valueRes = default)
        {
            return storeManager.GetCondition(store ,entity,type,value, dt,entityRes , typeRes , valueRes);
        }

        public ResponseT<Condition> AddCondition<T, M>(Guid store ,T entity, string type, double value,DateTime dt=default, M entityRes = default, string typeRes = default,
            double valueRes = default)
        {
            return storeManager.AddCondition(store , entity,type,value,dt,entityRes , typeRes , valueRes);
        }

        public void RemoveCondition<T, M>(Guid store ,T entity, string type, double value, DateTime dt=default, M entityRes = default, string typeRes = default,
            double valueRes = default)
        {
            storeManager.RemoveCondition(store ,entity,type,value,dt ,entityRes , typeRes , valueRes);
        }

        public ResponseT<Condition> AddDiscountCondition<T>(Guid store, T entity, string type, double value)
        {
            return storeManager.AddDiscountCondition(store, entity, type, value);
        }

        public ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid store, T level, int percent, DateTime startDate, DateTime endDate)
        {
            return storeManager.CreateSimplePolicy(store, level, percent, startDate , endDate);
        }

        public ResponseT<DiscountPolicy> CreateComplexPolicy(Guid store, string op, params object[] policys)
        {
            return storeManager.CreateComplexPolicy(store, op, policys);
        }

        public ResponseT<DiscountPolicyTree> AddPolicy(Guid store, DiscountPolicy discountPolicy)
        {
            return storeManager.AddPolicy(store, discountPolicy);
        }

        public void RemovePolicy(Guid store, DiscountPolicy discountPolicy)
        {
            storeManager.RemovePolicy(store, discountPolicy);
        }

        public void LoadData()
        {
            storeManager.LoadData();
        }

        public ResponseT<bool> IsAdmin(Guid userID)
        {
            return userManager.isAdmin(userID);
        }

        public ResponseT<Dictionary<Guid, SPermission>> GetMemberPermissions(Guid userID)
        {
            return userManager.GetMemberPermissions(userID);
        }

        public ResponseT<SStore> GetStoreInfo(Guid userID, Guid storeId)
        {
            return storeManager.GetStoreInfo(userID, storeId);
        }
    }
}