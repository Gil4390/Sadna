using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Win32;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
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
                if (testMode == true)
                    DatabaseContextFactory.TestMode = true;
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
            NotificationSystem.Instance.userFacade = userFacade;

            Setup();
        }
        
        public TradingSystem(IUserFacade userFacade, IStoreFacade storeFacade)
        {
            storeManager = new StoreManager(userFacade, storeFacade);
            userManager = new UserManager(userFacade);
            NotificationSystem.Instance.userFacade = userFacade;

            Setup();
        }

        public void Setup()
        {
            //load is system init
            bool isInit = DBHandler.Instance.LoadSystemInit();
            storeManager.SetIsSystemInitialize(isInit);
            userManager.SetIsSystemInitialize(isInit);

            // load all orders from db
            Orders.Instance.LoadOrdersFromDB();

            //load NotificationSystemList of Officials employees
            NotificationSystem.Instance.LoadNotificationOfficialsFromDB();

            //load founder list in userfacade ??

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
            DBHandler.Instance.SetSystemInit(isInitialize);
        }

        public int GetMaximumWaitServiceTime()
        {
            return 10000;
        }
        public ResponseT<Guid> Enter()
        {
            // load stores and items from data base
            //storeManager.LoadStoresFromDB();
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
                    SetIsSystemInitialize(true);
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
            try
            {
                ResponseT<Guid>  responseT = storeManager.OpenNewStore(userID, storeName);
                return responseT;

            } 
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
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
                double priceDiscount = storeManager.GetItemAfterDiscount(itemStoreid, item);
                if (!userManager.GetBidsOfUser(userID).ContainsKey(item.ItemID))
                    items.Add(new SItem(item, priceDiscount, itemStoreid, inStock, countInCart));
                else
                    items.Add(new SItem(item, priceDiscount, userManager.GetBidsOfUser(userID)[item.ItemID], itemStoreid, inStock, countInCart));
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

        public ResponseT<List<ItemForOrder>> PurchaseCart(Guid userID, SPaymentDetails paymentDetails, SSupplyDetails usersDetail)
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

        public ResponseT<List<SReview>> GetItemReviews(Guid itemID)
        {
            try
            {
                Logger.Instance.Info("getItemReviews on itemID: " + itemID);
                return storeManager.GetItemReviews(itemID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("error fetching reviews of item");
                return new ResponseT<List<SReview>>(ex.Message);
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

        public Response RemoveStoreManager(Guid userID1, Guid storeID, Guid userID2)
        {
            throw new NotImplementedException();
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
            try
            {
                ConcurrentDictionary<Guid, Member> members = userManager.GetMembers(userID);
                List<SMember> sMembers = new List<SMember>();

                foreach (Member member in members.Values)
                {
                    SMember sMember = new SMember(member);

                    if (member is PromotedMember)
                    {
                        PromotedMember pmember = (PromotedMember)member;

                        foreach (Guid storeID in pmember.Permission.Keys)
                        {
                            if (!storeID.Equals(Guid.Empty))
                            {
                                string storeName = storeManager.GetStore(storeID).Value.StoreName;
                                sMember.Permissions.Add(storeName + ": ");
                            }
                            sMember.Permissions.AddRange(pmember.Permission[storeID]);
                        }

                    }
                    sMembers.Add(sMember);
                }
                return new ResponseT<List<SMember>>(sMembers);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message + " in " + nameof(GetMembers));
                return new ResponseT<List<SMember>>(ex.Message);
            }
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
        public ResponseT<Member> GetMember(String email)
        {
            return userManager.GetMember(email);
        }
        public ResponseT<Store> GetStore(Guid storeID)
        {
            return storeManager.GetStore(storeID);
        }
        public ResponseT<Store> GetStore(String name)
        {
            return storeManager.GetStore(name);
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
        

        public void getNotificationsForOfflineMembers()
        {
            throw new NotImplementedException();
        }
        public bool IsSystemInitialize()
        {
            return userManager.IsSystemInitialize();
        }

        public ResponseT<SPolicy[]> GetAllConditions(Guid userID,Guid store)
        {
            return storeManager.GetAllConditions(userID,store);
        }

        public ResponseT<Condition> AddCondition(Guid userID,Guid store ,string entity, string entityName, string type, object value, DateTime dt=default, string entityRes = default,string entityResName=default,
            string typeRes = default, double valueRes = default , string op= default, int opCond= default)
        {
            return storeManager.AddCondition(userID,store , entity,entityName,type,value,dt,entityRes , entityResName, typeRes , valueRes  ,op , opCond);
        }

        public Response RemoveCondition(Guid userID,Guid storeID ,int condID)
        {
            return storeManager.RemoveCondition(userID,storeID,condID);
        }

        public ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid userID,Guid store, T level, int percent, DateTime startDate, DateTime endDate)
        {
            return storeManager.CreateSimplePolicy(userID,store, level, percent, startDate , endDate);
        }

        public ResponseT<DiscountPolicy> CreateComplexPolicy(Guid userID,Guid store, string op, params int[] policys)
        {
            return storeManager.CreateComplexPolicy(userID,store, op, policys);
        }

        public Response AddPolicy(Guid userID,Guid store, int discountPolicy)
        {
            return storeManager.AddPolicy(userID,store, discountPolicy);
        }

        public Response RemovePolicy(Guid userID,Guid store, int discountPolicy , string type)
        {
            return storeManager.RemovePolicy(userID,store, discountPolicy , type);
        }
        public ResponseT<List<SPolicy>> GetAllPolicy(Guid userID, Guid storeID)
        {
            return storeManager.GetAllPolicy(userID, storeID);
        }
        
        public void LoadData()
        {
            SetIsSystemInitialize(true);

            Guid systemManagerid = Enter().Value;
            Guid memberId = Enter().Value;
            Guid memberId2 = Enter().Value;
            Guid memberId3 = Enter().Value;
            Guid memberId4 = Enter().Value;
            Guid memberId5 = Enter().Value;
            Guid storeOwnerid1 = Enter().Value;
            Guid storeManagerid1 = Enter().Value;
            Guid storeOwnerid2 = Enter().Value;
            Guid storeManagerid2 = Enter().Value;

            Register(systemManagerid, ApplicationOptions.SystemManagerEmail, ApplicationOptions.SystemManagerFirstName, ApplicationOptions.SystemManagerLastName, ApplicationOptions.SystemManagerPass);
            Register(memberId, "gil@gmail.com", "Gil", "Gil", "asASD876!@");
            Register(memberId2, "sebatian@gmail.com", "Sebatian", "Sebatian", "asASD123!@");
            Register(memberId3, "amihai@gmail.com", "Amihai", "Amihai", "asASD123!@");
            Register(memberId4, "bar@gmail.com", "Bar", "Bar", "asASD159!@");
            Register(memberId5, "tal@gmail.com", "Tal", "Galmor", "w3ka!Tal");
            Register(storeOwnerid1, "AsiAzar@gmail.com", "Asi", "Azar", "A#!a12345678");
            Register(storeOwnerid2, "dani@gmail.com", "dani", "dani", "A#!a12345678");
            Register(storeManagerid1, "kobi@gmail.com", "kobi", "kobi", "A#!a12345678");
            Register(storeManagerid2, "Yael@gmail.com", "Yael", "Yael", "A#!a12345678");

            systemManagerid = Login(systemManagerid, ApplicationOptions.SystemManagerEmail, ApplicationOptions.SystemManagerPass).Value;
            userManager.CreateSystemManager(systemManagerid);

            storeOwnerid1 =Login(storeOwnerid1, "AsiAzar@gmail.com", "A#!a12345678").Value;
            Guid storeZaraID = OpenNewStore(storeOwnerid1, "Zara").Value;
            AddItemToStore(storeOwnerid1, storeZaraID, "Tshirt", "clothes", 99.8, 40);
            AddItemToStore(storeOwnerid1, storeZaraID, "Ipad", "electronic", 99.8, 2);
            AddItemToStore(storeOwnerid1, storeZaraID, "Dress", "clothes", 70, 45);

            AddCondition(storeOwnerid1, storeZaraID, "Item", "Tshirt", "min quantity", 2, DateTime.MaxValue);
            AddCondition(storeOwnerid1, storeZaraID, "Item", "Ipad", "min value", 1, DateTime.MaxValue);

            DiscountPolicy policy1 = CreateSimplePolicy(storeOwnerid1, storeZaraID, "StoreZara", 50, DateTime.Now, new DateTime(2024, 5, 20)).Value;
            Condition cond3 = AddCondition(storeOwnerid1, storeZaraID, "Item", "Tshirt", "min quantity", 2).Value;

            CreateComplexPolicy(storeOwnerid1, storeZaraID, "if", cond3.ID, policy1.ID);

            DiscountPolicy policy3 = CreateSimplePolicy(storeOwnerid1, storeZaraID, "StoreZara", 10, DateTime.Now, new DateTime(2024, 5, 20)).Value;

            AddPolicy(storeOwnerid1, storeZaraID, policy3.ID);

            AppointStoreManager(storeOwnerid1, storeZaraID, "kobi@gmail.com");

            storeOwnerid2 = Login(storeOwnerid2, "dani@gmail.com", "A#!a12345678").Value;
            Guid storeFoxID = OpenNewStore(storeOwnerid2, "Fox").Value;
            AddItemToStore(storeOwnerid2, storeFoxID, "Pants", "clothes", 150, 200);
            AddItemToStore(storeOwnerid2, storeFoxID, "Towel", "Home", 40, 450);
            AddItemToStore(storeOwnerid2, storeFoxID, "Teddy bear toy", "children toys", 65, 120);
            AddItemToStore(storeOwnerid2, storeFoxID, "mouse", "animals", 65, 0);

            AppointStoreManager(storeOwnerid2, storeFoxID, "Yael@gmail.com");

            Exit(systemManagerid);
            Exit(memberId);
            Exit(memberId2);
            Exit(memberId3);
            Exit(memberId4);
            Exit(memberId5);
            Exit(storeOwnerid1);
            Exit(storeManagerid1);
            Exit(storeOwnerid2);
            Exit(storeManagerid2);

            //SetIsSystemInitialize(false);
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

        public Response MarkNotificationAsRead(Guid userID, Guid notificationID)
        {
            return userManager.MarkNotificationAsRead(userID, notificationID);
        }

        public Response CheckPurchaseConditions(Guid userID)
        {
            return storeManager.CheckPurchaseConditions(userID);
        }

        public ResponseT<string> GetMemberName(Guid userID)
        {
            String name = userManager.GetMember(userID).Value.FirstName;
            var res = new ResponseT<String>();
            res.Value = name;
            return res;
        }

        public ResponseT<double> GetStoreRevenue(Guid userID, Guid storeID, DateTime date)
        {
            return storeManager.GetStoreRevenue(userID, storeID, date);
        }

        public ResponseT<double> GetSystemRevenue(Guid userID, DateTime date)
        {
            return storeManager.GetSystemRevenue(userID,date);
        }

        public Response Handshake()
        {
            return userManager.Handshake();
        }

        public ResponseT<SBid> PlaceBid(Guid userID, Guid itemID, double price)
        {
            return storeManager.PlaceBid(userID, itemID, price);
        }

        public ResponseT<SBid[]> GetBidsInStore(Guid userID, Guid storeID)
        {
            return userManager.GetBidsInStore(userID, storeID);
        }

        public Response ReactToBid(Guid userID, Guid itemID, Guid bidID, string bidResponse)
        {
            return storeManager.ReactToBid(userID, itemID, bidID, bidResponse);
        }

        public void CreateSystemManager(Guid systemManagerid)
        {
            userManager.CreateSystemManager(systemManagerid);
        }
    }
}