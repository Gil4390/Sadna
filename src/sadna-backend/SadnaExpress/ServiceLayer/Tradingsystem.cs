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

        #region Constructor
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
        #endregion

        public void Setup()
        {
            try
            {
                //load is system init
                bool isInit = DBHandler.Instance.LoadSystemInit();
                storeManager.SetIsSystemInitialize(isInit);
                userManager.SetIsSystemInitialize(isInit);

                // load all orders from db
                Orders.Instance.LoadOrdersFromDB();

                //load NotificationSystemList of Officials employees
                NotificationSystem.Instance.LoadNotificationOfficialsFromDB();

                //load visits list in UserUsageData 
                UserUsageData.Instance.LoadVisitsData();
            }
            catch(Exception ex)
            {
                Logger.Instance.Info($"Trading system did not initialized correctly: {ex.Message}");
            }
        }

        #region System init
        public bool IsSystemInitialize()
        {
            return userManager.IsSystemInitialize();
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            userManager.SetIsSystemInitialize(isInitialize);
            storeManager.SetIsSystemInitialize(isInitialize);
            DBHandler.Instance.SetSystemInit(isInitialize);
        }
        #endregion

        #region User operations
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

        public ResponseT<bool> IsAdmin(Guid userID)
        {
            return userManager.isAdmin(userID);
        }

        #endregion

        #region Admin operations

        public ResponseT<bool> InitializeTradingSystem(Guid userID)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
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

        public ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID)
        {
            return storeManager.GetAllStorePurchases(userID);
        }

        public Response RemoveUserMembership(Guid userID, string email)
        {
            return userManager.RemoveUserMembership(userID, email);
        }

        public ResponseT<List<SMember>> GetMembers(Guid userID)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
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

        public ResponseT<Dictionary<Guid, List<Order>>> GetPurchasesInfoUser(Guid userID)
        {
            Dictionary<Guid, List<Order>> orders = Orders.Instance.GetUserOrders();

            return new ResponseT<Dictionary<Guid, List<Order>>>(orders);
        }

        public ResponseT<double> GetSystemRevenue(Guid userID, DateTime date)
        {
            return storeManager.GetSystemRevenue(userID, date);
        }

        public ResponseT<List<int>> GetSystemUserActivity(Guid userID, DateTime fromDate, DateTime toDate)
        {
            // guests, members, managers, owners, admins

            return userManager.GetSystemUserActivity(userID, fromDate, toDate);
        }

        public void CreateSystemManager(Guid systemManagerid)
        {
            userManager.CreateSystemManager(systemManagerid);
        }

        #endregion

        #region User shopping operations
        public ResponseT<List<SItem>> GetItemsForClient(Guid userID, string keyWords, int minPrice = 0,
            int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
            }
            catch(Exception ex)
            {
                Logger.Instance.Error(userID, nameof(TradingSystem) + ": " + nameof(GetItemsForClient) + ": " + ex.Message);
                return new ResponseT<List<SItem>>(new List<SItem>());
            }
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

        public ResponseT<List<SItem>> GetCartItems(Guid userID)
        {
            return storeManager.GetCartItems(userID);
        }

        public ResponseT<List<ItemForOrder>> PurchaseCart(Guid userID, SPaymentDetails paymentDetails, SSupplyDetails usersDetail)
        {
            ResponseT<List<ItemForOrder>>  response = storeManager.PurchaseCart(userID, paymentDetails, usersDetail);
            return response;
        }

        public Response CheckPurchaseConditions(Guid userID)
        {
            return storeManager.CheckPurchaseConditions(userID);
        }

        #endregion

        #region User data
        public ResponseT<List<ItemForOrder>> GetPurchasesOfUser(Guid userID)
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

        public ResponseT<List<Notification>> GetNotifications(Guid userID)
        {
            return userManager.GetNotifications(userID);
        }

        public Response MarkNotificationAsRead(Guid userID, Guid notificationID)
        {
            return userManager.MarkNotificationAsRead(userID, notificationID);
        }

        public ResponseT<Dictionary<Guid, SPermission>> GetMemberPermissions(Guid userID)
        {
            return userManager.GetMemberPermissions(userID);
        }

        public ResponseT<string> GetMemberName(Guid userID)
        {
            String name = userManager.GetMember(userID).Value.FirstName;
            var res = new ResponseT<String>();
            res.Value = name;
            return res;
        }
        #endregion

        #region Reviews

        public Response WriteItemReview(Guid userID, Guid itemID, string review)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
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
                DBHandler.Instance.CanConnectToDatabase();
                Logger.Instance.Info("getItemReviews on itemID: " + itemID);
                return storeManager.GetItemReviews(itemID);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("error fetching reviews of item");
                return new ResponseT<List<SReview>>(ex.Message);
            }
        }

        #endregion

        #region Store manage
        public ResponseT<Guid> OpenNewStore(Guid userID, string storeName)
        {
            try
            {
                DBHandler.Instance.CanConnectToDatabase();
                ResponseT<Guid> responseT = storeManager.OpenNewStore(userID, storeName);
                return responseT;

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<Guid>(ex.Message);
            }
        }

        public ResponseT<SStore> GetStoreInfo(Guid userID, Guid storeId)
        {
            return storeManager.GetStoreInfo(userID, storeId);
        }

        public ResponseT<Guid> AddItemToStore(Guid userID, Guid storeID,  string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return storeManager.AddItemToStore(userID, storeID, itemName, itemCategory, itemPrice, quantity);
        }

        public Response RemoveItemFromStore(Guid userID, Guid storeID, Guid itemID)
        {
            return storeManager.RemoveItemFromStore(userID, storeID, itemID);
        }

        public Response EditItem(Guid userID, Guid storeID, Guid itemID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return storeManager.EditItem(userID, storeID, itemID, itemName, itemCategory, itemPrice, quantity);
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

        public Response RemovePermission(Guid userID, Guid storeID, string userEmail, string permission)
        {
            return userManager.RemovePermission(userID, storeID, userEmail, permission);
        }

        public Response CloseStore(Guid userID, Guid storeID)
        {
            Response response = storeManager.CloseStore(userID, storeID);
            return response;
        }

        public ResponseT<List<SMemberForStore>> GetEmployeeInfoInStore(Guid userID, Guid storeID)
        {
            return userManager.GetEmployeeInfoInStore(userID, storeID);
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

        public ResponseT<List<Item>> GetItemsInStore(Guid userID, Guid storeId)
        {
            return storeManager.GetItemsInStore(userID, storeId);
        }

        public ResponseT<double> GetStoreRevenue(Guid userID, Guid storeID, DateTime date)
        {
            return storeManager.GetStoreRevenue(userID, storeID, date);
        }

        public ResponseT<SBid[]> GetBidsInStore(Guid userID, Guid storeID)
        {
            return userManager.GetBidsInStore(userID, storeID);
        }

        #region Policy & Condition

        public ResponseT<SPolicy[]> GetAllConditions(Guid userID, Guid store)
        {
            return storeManager.GetAllConditions(userID, store);
        }

        public ResponseT<Condition> AddCondition(Guid userID, Guid store, string entity, string entityName, string type, object value, DateTime dt = default, string entityRes = default, string entityResName = default,
            string typeRes = default, double valueRes = default, string op = default, int opCond = default)
        {
            return storeManager.AddCondition(userID, store, entity, entityName, type, value, dt, entityRes, entityResName, typeRes, valueRes, op, opCond);
        }

        public Response RemoveCondition(Guid userID, Guid storeID, int condID)
        {
            return storeManager.RemoveCondition(userID, storeID, condID);
        }

        public ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid userID, Guid store, T level, int percent, DateTime startDate, DateTime endDate)
        {
            return storeManager.CreateSimplePolicy(userID, store, level, percent, startDate, endDate);
        }

        public ResponseT<DiscountPolicy> CreateComplexPolicy(Guid userID, Guid store, string op, params int[] policys)
        {
            return storeManager.CreateComplexPolicy(userID, store, op, policys);
        }

        public Response AddPolicy(Guid userID, Guid store, int discountPolicy)
        {
            return storeManager.AddPolicy(userID, store, discountPolicy);
        }

        public Response RemovePolicy(Guid userID, Guid store, int discountPolicy, string type)
        {
            return storeManager.RemovePolicy(userID, store, discountPolicy, type);
        }

        public ResponseT<List<SPolicy>> GetAllPolicy(Guid userID, Guid storeID)
        {
            return storeManager.GetAllPolicy(userID, storeID);
        }

        #endregion

        #endregion

        #region Bids
        public ResponseT<SBid> PlaceBid(Guid userID, Guid itemID, double price)
        {
            return storeManager.PlaceBid(userID, itemID, price);
        }

        public Response ReactToBid(Guid userID, Guid itemID, Guid bidID, string bidResponse)
        {
            return storeManager.ReactToBid(userID, itemID, bidID, bidResponse);
        }

        public Response ReactToJobOffer(Guid userID, Guid storeID, Guid newEmpID, bool offerResponse)
        {
            return userManager.ReactToJobOffer(userID, storeID, newEmpID, offerResponse);
        }

        #endregion

        #region Getters & Setters
        public ResponseT<List<Store>> GetAllStoreInfo()
        {
            try
            {
                Logger.Instance.Info($"{nameof(TradingSystem)} {nameof(GetAllStoreInfo)} has started");
                return storeManager.GetAllStoreInfo();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<List<Store>>(ex.Message);
            }
        }

        public ResponseT<ShoppingCart> GetDetailsOnCart(Guid userID)
        {
            return storeManager.GetDetailsOnCart(userID);
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

        #endregion

        public void LoadData()
        {

            SetIsSystemInitialize(true);

            #region Enter
            ResponseT<Guid> enterResSystemManager = Enter();
            if (enterResSystemManager.ErrorOccured)
                ThrowLoadDataException(enterResSystemManager.ErrorMessage);
            Guid systemManagerid = enterResSystemManager.Value;

            ResponseT<Guid> enterResMember = Enter();
            if (enterResMember.ErrorOccured)
                ThrowLoadDataException(enterResMember.ErrorMessage);
            Guid memberId = enterResMember.Value;

            ResponseT<Guid> enterResMember2 = Enter();
            if (enterResMember2.ErrorOccured)
                ThrowLoadDataException(enterResMember2.ErrorMessage);
            Guid memberId2 = enterResMember2.Value;

            ResponseT<Guid> enterResMember3 = Enter();
            if (enterResMember3.ErrorOccured)
                ThrowLoadDataException(enterResMember3.ErrorMessage);
            Guid memberId3 = enterResMember3.Value;

            ResponseT<Guid> enterResMember4 = Enter();
            if (enterResMember4.ErrorOccured)
                ThrowLoadDataException(enterResMember4.ErrorMessage);
            Guid memberId4 = enterResMember4.Value;

            ResponseT<Guid> enterResMember5 = Enter();
            if (enterResMember5.ErrorOccured)
                ThrowLoadDataException(enterResMember5.ErrorMessage);
            Guid memberId5 = enterResMember5.Value;

            ResponseT<Guid> enterResStoreOwner1 = Enter();
            if (enterResStoreOwner1.ErrorOccured)
                ThrowLoadDataException(enterResStoreOwner1.ErrorMessage);
            Guid storeOwnerid1 = enterResStoreOwner1.Value;

            ResponseT<Guid> enterResStoreManager1 = Enter();
            if (enterResStoreManager1.ErrorOccured)
                ThrowLoadDataException(enterResStoreManager1.ErrorMessage);
            Guid storeManagerid1 = enterResStoreManager1.Value;

            ResponseT<Guid> enterResStoreOwner2 = Enter();
            if (enterResStoreOwner2.ErrorOccured)
                ThrowLoadDataException(enterResStoreOwner2.ErrorMessage);
            Guid storeOwnerid2 = enterResStoreOwner2.Value;

            ResponseT<Guid> enterResStoreManager2 = Enter();
            if (enterResStoreManager2.ErrorOccured)
                ThrowLoadDataException(enterResStoreManager2.ErrorMessage);
            Guid storeManagerid2 = enterResStoreManager2.Value;
            #endregion

            #region Register

            Response resRegisterSystemManager = Register(systemManagerid, ApplicationOptions.SystemManagerEmail, ApplicationOptions.SystemManagerFirstName, ApplicationOptions.SystemManagerLastName, ApplicationOptions.SystemManagerPass);
            if (resRegisterSystemManager.ErrorOccured) { ThrowLoadDataException(resRegisterSystemManager.ErrorMessage); }

            Response resRegisterMember = Register(memberId, "gil@gmail.com", "Gil", "Gil", "asASD876!@");
            if (resRegisterMember.ErrorOccured) { ThrowLoadDataException(resRegisterMember.ErrorMessage); }

            Response resRegisterMember2 = Register(memberId2, "sebatian@gmail.com", "Sebatian", "Sebatian", "asASD123!@");
            if (resRegisterMember2.ErrorOccured) { ThrowLoadDataException(resRegisterMember2.ErrorMessage); }

            Response resRegisterMember3 = Register(memberId3, "amihai@gmail.com", "Amihai", "Amihai", "asASD123!@");
            if (resRegisterMember3.ErrorOccured) { ThrowLoadDataException(resRegisterMember3.ErrorMessage); }

            Response resRegisterMember4 = Register(memberId4, "bar@gmail.com", "Bar", "Bar", "asASD159!@");
            if (resRegisterMember4.ErrorOccured) { ThrowLoadDataException(resRegisterMember4.ErrorMessage); }

            Response resRegisterMember5 = Register(memberId5, "tal@gmail.com", "Tal", "Galmor", "w3ka!Tal");
            if (resRegisterMember5.ErrorOccured) { ThrowLoadDataException(resRegisterMember5.ErrorMessage); }

            Response resRegisterStoreOwner1 = Register(storeOwnerid1, "AsiAzar@gmail.com", "Asi", "Azar", "A#!a12345678");
            if (resRegisterStoreOwner1.ErrorOccured) { ThrowLoadDataException(resRegisterStoreOwner1.ErrorMessage); }

            Response resRegisterStoreOwner2 = Register(storeOwnerid2, "dani@gmail.com", "dani", "dani", "A#!a12345678");
            if (resRegisterStoreOwner2.ErrorOccured) { ThrowLoadDataException(resRegisterStoreOwner2.ErrorMessage); }

            Response resRegisterStoreManager1 = Register(storeManagerid1, "kobi@gmail.com", "kobi", "kobi", "A#!a12345678");
            if (resRegisterStoreManager1.ErrorOccured) { ThrowLoadDataException(resRegisterStoreManager1.ErrorMessage); }

            Response resRegisterStoreManager2 = Register(storeManagerid2, "Yael@gmail.com", "Yael", "Yael", "A#!a12345678");
            if (resRegisterStoreManager2.ErrorOccured) { ThrowLoadDataException(resRegisterStoreManager2.ErrorMessage); }

            #endregion

            #region system manager actions
            ResponseT<Guid> resLoginSystemManager = Login(systemManagerid, ApplicationOptions.SystemManagerEmail, ApplicationOptions.SystemManagerPass);
            if (resLoginSystemManager.ErrorOccured) { ThrowLoadDataException(resLoginSystemManager.ErrorMessage); }
            systemManagerid = resLoginSystemManager.Value;
            userManager.CreateSystemManager(systemManagerid);
            #endregion

            #region store owner 1 actions
            ResponseT<Guid> resLoginStoreOwner1 = Login(storeOwnerid1, "AsiAzar@gmail.com", "A#!a12345678");
            if (resLoginStoreOwner1.ErrorOccured) { ThrowLoadDataException(resLoginStoreOwner1.ErrorMessage); }
            storeOwnerid1 = resLoginStoreOwner1.Value;

            ResponseT<Guid> resOpenNewStoreStoreOwner1 = OpenNewStore(storeOwnerid1, "Zara");
            if (resOpenNewStoreStoreOwner1.ErrorOccured) { ThrowLoadDataException(resOpenNewStoreStoreOwner1.ErrorMessage); }
            Guid storeZaraID = resOpenNewStoreStoreOwner1.Value;

            ResponseT<Guid> resAddItemToStoreTshirt = AddItemToStore(storeOwnerid1, storeZaraID, "Tshirt", "clothes", 99.8, 40);
            if (resAddItemToStoreTshirt.ErrorOccured) { ThrowLoadDataException(resAddItemToStoreTshirt.ErrorMessage); }

            ResponseT<Guid> resAddItemToStoreIpad = AddItemToStore(storeOwnerid1, storeZaraID, "Ipad", "electronic", 99.8, 2);
            if (resAddItemToStoreIpad.ErrorOccured) { ThrowLoadDataException(resAddItemToStoreIpad.ErrorMessage); }

            ResponseT<Guid> resAddItemToStoreDress = AddItemToStore(storeOwnerid1, storeZaraID, "Dress", "clothes", 70, 45);
            if (resAddItemToStoreDress.ErrorOccured) { ThrowLoadDataException(resAddItemToStoreDress.ErrorMessage); }

            ResponseT<Condition> resAddCondition1 = AddCondition(storeOwnerid1, storeZaraID, "Item", "Tshirt", "min quantity", 2, DateTime.MaxValue);
            if (resAddCondition1.ErrorOccured) { ThrowLoadDataException(resAddCondition1.ErrorMessage); }

            ResponseT<Condition> resAddCondition2 = AddCondition(storeOwnerid1, storeZaraID, "Item", "Ipad", "min value", 1, DateTime.MaxValue);
            if (resAddCondition2.ErrorOccured) { ThrowLoadDataException(resAddCondition2.ErrorMessage); }

            ResponseT<DiscountPolicy> resCreateSimplePolicy1 = CreateSimplePolicy(storeOwnerid1, storeZaraID, "StoreZara", 50, DateTime.Now, new DateTime(2024, 5, 20));
            if (resCreateSimplePolicy1.ErrorOccured) { ThrowLoadDataException(resCreateSimplePolicy1.ErrorMessage); }
            DiscountPolicy policy1 = resCreateSimplePolicy1.Value;

            ResponseT<Condition> resAddCondition3 = AddCondition(storeOwnerid1, storeZaraID, "Item", "Tshirt", "min quantity", 2);
            if (resAddCondition3.ErrorOccured) { ThrowLoadDataException(resAddCondition3.ErrorMessage); }
            Condition cond3 = resAddCondition3.Value;

            ResponseT<DiscountPolicy> resCreateComplexPolicy1 = CreateComplexPolicy(storeOwnerid1, storeZaraID, "if", cond3.ID, policy1.ID);
            if (resCreateComplexPolicy1.ErrorOccured) { ThrowLoadDataException(resCreateComplexPolicy1.ErrorMessage); }

            ResponseT<DiscountPolicy> resCreateSimplePolicy3 = CreateSimplePolicy(storeOwnerid1, storeZaraID, "StoreZara", 10, DateTime.Now, new DateTime(2024, 5, 20));
            if (resCreateSimplePolicy3.ErrorOccured) { ThrowLoadDataException(resCreateSimplePolicy3.ErrorMessage); }
            DiscountPolicy policy3 = resCreateSimplePolicy3.Value;

            Response resAddPolicy = AddPolicy(storeOwnerid1, storeZaraID, policy3.ID);
            if (resAddPolicy.ErrorOccured) { ThrowLoadDataException(resAddPolicy.ErrorMessage); }

            Response resAppointStoreManager1 = AppointStoreManager(storeOwnerid1, storeZaraID, "kobi@gmail.com");
            if (resAppointStoreManager1.ErrorOccured) { ThrowLoadDataException(resAppointStoreManager1.ErrorMessage); }
            #endregion

            #region store owner 2 actions
            ResponseT<Guid> resLoginStoreOwner2 = Login(storeOwnerid2, "dani@gmail.com", "A#!a12345678");
            if (resLoginStoreOwner2.ErrorOccured) { ThrowLoadDataException(resLoginStoreOwner2.ErrorMessage); }
            storeOwnerid2 = resLoginStoreOwner2.Value;

            ResponseT<Guid> resOpenNewStoreStoreOwner2 = OpenNewStore(storeOwnerid2, "Fox");
            if (resOpenNewStoreStoreOwner2.ErrorOccured) { ThrowLoadDataException(resOpenNewStoreStoreOwner2.ErrorMessage); }
            Guid storeFoxID = resOpenNewStoreStoreOwner2.Value;

            ResponseT<Guid> resAddItemToStorePants = AddItemToStore(storeOwnerid2, storeFoxID, "Pants", "clothes", 150, 200);
            if (resAddItemToStorePants.ErrorOccured) { ThrowLoadDataException(resAddItemToStorePants.ErrorMessage); }

            ResponseT<Guid> resAddItemToStoreTowel = AddItemToStore(storeOwnerid2, storeFoxID, "Towel", "Home", 40, 450);
            if (resAddItemToStoreTowel.ErrorOccured) { ThrowLoadDataException(resAddItemToStoreTowel.ErrorMessage); }

            ResponseT<Guid> resAddItemToStoreTeddyBearToy = AddItemToStore(storeOwnerid2, storeFoxID, "Teddy bear toy", "children toys", 65, 120);
            if (resAddItemToStoreTeddyBearToy.ErrorOccured) { ThrowLoadDataException(resAddItemToStoreTeddyBearToy.ErrorMessage); }

            ResponseT<Guid> resAddItemToStoreMouse = AddItemToStore(storeOwnerid2, storeFoxID, "mouse", "animals", 65, 0);
            if (resAddItemToStoreMouse.ErrorOccured) { ThrowLoadDataException(resAddItemToStoreMouse.ErrorMessage); }

            Response resAppointStoreManager2 = AppointStoreManager(storeOwnerid2, storeFoxID, "Yael@gmail.com");
            if (resAppointStoreManager2.ErrorOccured) { ThrowLoadDataException(resAppointStoreManager2.ErrorMessage); }
            #endregion

            #region Exit all users
            Response resExitSystemManager = Exit(systemManagerid);
            if (resExitSystemManager.ErrorOccured) { ThrowLoadDataException(resExitSystemManager.ErrorMessage); }

            Response resExitMember = Exit(memberId);
            if (resExitMember.ErrorOccured) { ThrowLoadDataException(resExitMember.ErrorMessage); }

            Response resExitMember2 = Exit(memberId2);
            if (resExitMember2.ErrorOccured) { ThrowLoadDataException(resExitMember2.ErrorMessage); }

            Response resExitMember3 = Exit(memberId3);
            if (resExitMember3.ErrorOccured) { ThrowLoadDataException(resExitMember3.ErrorMessage); }

            Response resExitMember4 = Exit(memberId4);
            if (resExitMember4.ErrorOccured) { ThrowLoadDataException(resExitMember4.ErrorMessage); }

            Response resExitMember5 = Exit(memberId5);
            if (resExitMember5.ErrorOccured) { ThrowLoadDataException(resExitMember5.ErrorMessage); }

            Response resExitStoreOwner1 = Exit(storeOwnerid1);
            if (resExitStoreOwner1.ErrorOccured) { ThrowLoadDataException(resExitStoreOwner1.ErrorMessage); }

            Response resExitStoreManager1 = Exit(storeManagerid1);
            if (resExitStoreManager1.ErrorOccured) { ThrowLoadDataException(resExitStoreManager1.ErrorMessage); }

            Response resExitStoreOwner2 = Exit(storeOwnerid2);
            if (resExitStoreOwner2.ErrorOccured) { ThrowLoadDataException(resExitStoreOwner2.ErrorMessage); }

            Response resExitStoreManager2 = Exit(storeManagerid2);
            if (resExitStoreManager2.ErrorOccured) { ThrowLoadDataException(resExitStoreManager2.ErrorMessage); }

            #endregion

            SetIsSystemInitialize(ApplicationOptions.InitTradingSystem);
        }

        public void CleanUp() // for the tests
        {
            storeManager.CleanUp();
            userManager.CleanUp();
        }

        private static void ThrowLoadDataException(string errMsg)
        {
            Logger.Instance.Error($"Load data failed, system can not load, error msg: {errMsg}");
            throw new Exception($"Load data failed, system can not load, error msg: {errMsg}");

        }
    }
}