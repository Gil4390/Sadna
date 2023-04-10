using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;

using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Timer = System.Timers.Timer;
using static SadnaExpress.DomainLayer.Store.DiscountPolicy;
using System.Collections;
using System.Diagnostics;
using System.Net;

namespace SadnaExpress.ServiceLayer
{
    public class TradingSystem : ITradingSystem
    {
        private ISupplierService supplierService;
        private IPaymentService paymentService;
        private static IStoreManager storeManager;
        private static IUserManager userManager;
        private const int ExternalServiceWaitTimeInSeconds=10000; //10 seconds is 10,000 mili seconds
        public IPaymentService PaymentService { get => paymentService; set => paymentService = value; }
        public ISupplierService SupplierService { get => supplierService; set => supplierService = value; }

        // lock object for saving items 
        private static readonly object stockChange = new object();
        // fields for the saved items
        // list for saved items  <userId   <itemsId selected in Cart>>
        private static Dictionary<int, List<Pair<Guid, List<int>>>> savedItems = new Dictionary<int, List<Pair<Guid, List<int>>>>();
        private static Dictionary<int, Dictionary<Guid, List<Pair<int, int>>>> savedItemsRestore = new Dictionary<int, Dictionary<Guid, List<Pair<int, int>>>>();
        private static Dictionary<int, Timer> savedTimer = new Dictionary<int, Timer>();
        private static Dictionary<int, bool> savedPurchaseResult = new Dictionary<int, bool>();
        private static Queue<int> savedItemsUserId = new Queue<int>();


        // lock object for the instance
        private static readonly object lockInstance = new object();
        private static TradingSystem instance = null;
        
        private TradingSystem(ISupplierService supplierService=null, IPaymentService paymentService=null)
        {
            userManager = new UserManager();
            //storeManager = new StoreManager();
            storeManager = new StoreManager(userManager.GetUserFacade());

            this.paymentService = paymentService;
            this.supplierService = supplierService;
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

        public int GetAvailableQuantity(Guid storeId, int itemId)
        {
            return storeManager.GetStoreById(storeId).getItemsInventory().getItemQuantityById(itemId);
        }


        public int GetMaximumWaitServiceTime()
        {
            return ExternalServiceWaitTimeInSeconds;
        }
        public ResponseT<int> Enter()
        {
            return userManager.Enter();
        }
        public Response Exit(int id)
        {
            return userManager.Exit(id);
        }

        public Response Register(int id, string email, string firstName, string lastName, string password)
        {
            return userManager.Register(id, email, firstName, lastName, password);
        }

        public ResponseT<int> Login(int id, string email, string password)
        {
            return userManager.Login(id, email, password);
        }

        public ResponseT<int> Logout(int id)
        {
            return userManager.Logout(id);
        }

        public ResponseT<List<S_Store>> GetAllStoreInfo(int id)
        {
            //get list of business stores and convert them to service stores
            throw new NotImplementedException();
        }

        public ResponseT<Guid> OpenNewStore(int id, string storeName)
        {
            return storeManager.OpenNewStore(id, storeName);
        }
        
        public ResponseT<List<S_Item>> GetItemsByName(int id, string itemName)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByCategory(int id, string category)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByKeysWord(int id, string keyWords)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByPrices(int id, int minPrice, int maxPrice)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByItemRating(int id, int rating)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<S_Item>> GetItemsByStoreRating(int id, int rating)
        {
            throw new NotImplementedException();
        }

        public Response AddItemToCart(int id, Guid storeID, int itemID, int itemAmount)
        {
            return userManager.AddItemToCart(id, storeID, itemID, itemAmount);
        }

        public Response RemoveItemFromCart(int id, Guid storeID, int itemID)
        {
            return userManager.RemoveItemFromCart(id, storeID, itemID);
        }

        public Response EditItemFromCart(int id, int itemID, int itemAmount)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Dictionary<string, List<string>>> getDetailsOnCart()
        {
            throw new NotImplementedException();
        }

        public Response WriteReview(int id, int itemID, string review)
        {
            throw new NotImplementedException();
        }

        public Response RateItem(int id, int itemID, int score)
        {
            throw new NotImplementedException();
        }

        public Response WriteMessageToStore(int id, Guid storeID, string message)
        {
            throw new NotImplementedException();
        }

        public Response ComplainToAdmin(int id, string message)
        {
            throw new NotImplementedException();
        }

        public Response GetPurchasesInfo(int id)
        {
            throw new NotImplementedException();
        }

        public Response AddItemToStore(int id, Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            // TODO : todo check if user can add item to this store
            try
            {
                bool hasPemissionToAddItem = true;
                if (!hasPemissionToAddItem)
                    throw new Exception("Cannot add item to store, userid: " + id + " don't have pemission to add items");

                //if (!storeManager.GetStoreById(storeID).addItem(itemName, itemCategory, itemPrice, quantity))
                //{
                //    throw new Exception("Failed to add new item to store");
                //}

                storeManager.AddItemToStore(storeID, itemName, itemCategory, itemPrice, quantity);


                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response();
            }

        }

        public Response RemoveItemFromStore(int id, Guid storeID, int itemID)
        {
            try
            {
                bool hasPemissionToAddItem = true;
                if (!hasPemissionToAddItem)
                    throw new Exception("Cannot add item to store, userid: " + id + " don't have pemission to add items");

                //if (!storeManager.GetStoreById(storeID).addItem(itemName, itemCategory, itemPrice, quantity))
                //{
                //    throw new Exception("Failed to add new item to store");
                //}

                storeManager.RemoveItemFromStore(storeID, itemID);


                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new Response();
            }
        }

        public Response EditItemCategory(string storeName, string itemName, string category)
        {
            throw new NotImplementedException();
        }

        public Response EditItemPrice(string storeName, string itemName, int price)
        {
            throw new NotImplementedException();
        }


        public Response AppointStoreOwner(int id, Guid storeID, string userEmail)
        {
            return userManager.AppointStoreOwner(id, storeID, userEmail);
        }

        public Response AppointStoreManager(int id, Guid storeID, string userEmail)
        {
            return userManager.AppointStoreOwner(id, storeID, userEmail);
        }

        public Response AddStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission)
        {
            return userManager.AddStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission)
        {
            return userManager.RemoveStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManager(int id, Guid storeID, int userID)
        {
            throw new NotImplementedException();
        }

        public Response RemoveStoreOwner(int id, Guid storeID, int userID)
        {
            throw new NotImplementedException();
        }

        public Response RemovetStoreManager(int id, Guid storeID, int userID)
        {
            throw new NotImplementedException();
        }

        public Response CloseStore(int id, Guid storeID)
        {
            return storeManager.CloseStore(id,storeID);
        }

        public Response ReopenStore(int id, Guid storeID)
        {
            return storeManager.ReopenStore(id,storeID);
        }

        public ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, Guid storeID)
        {
            return userManager.GetEmployeeInfoInStore(id, storeID);
        }

        public Response GetPurchasesInfo(int id, Guid storeID)
        {
            throw new NotImplementedException();
        }

        public Response DeleteStore(int id, Guid storeID)
        {
            return storeManager.DeleteStore(id,storeID);
        }

        public bool CheckSupplierConnection()
        {
            bool result = this.supplierService.Connect();
            if (!result)
            {
                Logger.Instance.Error("Supplier Service Connection Failed");
            }
            return result;
        }

        public Response DeleteMember(int id, int userID)
        {
            throw new NotImplementedException();
        }

        public ResponseT<int> UpdateFirst(int id, string newFirst)
        {
            return storeManager.UpdateFirst(id, newFirst);
        }

        public ResponseT<int> UpdateLast(int id, string newLast)
        {
            return storeManager.UpdateLast(id, newLast);
        }

        public ResponseT<int> UpdatePassword(int id, string newPassword)
        {
            return storeManager.UpdatePassword(id, newPassword);
        }

        public ResponseT<int> SetSecurityQA(int id,string q, string a)
        {
            return userManager.SetSecurityQA(id,q,a);
        }

        public bool CheckPaymentConnection()
        {
            bool result = this.paymentService.Connect();
            if (!result)
            {
                Logger.Instance.Error("Payment Service Connection Failed");
            }
            return result;
        }

        public void CleanUp() // for the tests
        {
            storeManager.CleanUp();
            userManager.CleanUp();
            supplierService = null;
            paymentService = null;
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public ResponseT<bool> PlacePayment(string transactionDetails)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return paymentService.ValidatePayment(transactionDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(ExternalServiceWaitTimeInSeconds));

                if (isCompletedSuccessfully)
                {
                    return new ResponseT<bool>(task.Result);
                }
                else
                {
                    throw new TimeoutException("Payment external service action has taken longer than the maximum time allowed.");
                }
            }
            catch(Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<bool> PlaceSupply(string orderDetails, string userDetails)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return supplierService.ShipOrder(orderDetails,userDetails);
                });

                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(ExternalServiceWaitTimeInSeconds));

                if (isCompletedSuccessfully)
                {
                    return new ResponseT<bool>(task.Result);
                }
                else
                {
                    throw new TimeoutException("Supply external service action has taken longer than the maximum time allowed.");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<bool> InitializeTradingSystem(int id)
        {
            try
            {
                Logger.Instance.Info("User id: " + id + " requested to initialize trading system");
                userManager.InitializeTradingSystem(id);
                return new ResponseT<bool>(paymentService.Connect() && supplierService.Connect());
            }
            catch(Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<ShoppingCart> ShowShoppingCart(int id)
        {
            return userManager.ShowShoppingCart(id);
        }

        public ConcurrentDictionary<int , User> GetCurrent_Users()
        {
            return userManager.GetCurrent_Users();
        }
        public ConcurrentDictionary<int , Member> GetMembers()
        {
            return userManager.GetMembers();
        }
        public ConcurrentDictionary<Guid , Store> GetStores()
        {
            return storeManager.GetStores();
        }


        /////////////////////////////////////////// function for saving items for users
        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // check after timer passes
            lock (stockChange)
            {
                int id = savedItemsUserId.Dequeue();
                if (savedPurchaseResult[id] == false)
                {
                    // need to restore stock failed Purchase
                    foreach (Guid storeId in savedItemsRestore[id].Keys)
                    {
                        foreach (Pair<int, int> storeItem in savedItemsRestore[id][storeId])
                        {
                            storeManager.GetStoreById(storeId).AddQuantity(storeItem.First, storeItem.Second);
                        }
                    }
                    Logger.Instance.Error("No Purchase Success after saving items for 2 minutes!");
                }
                savedItemsRestore[id] = new Dictionary<Guid, List<Pair<int, int>>>();


            }
        }

        /////////////////////////////////////////////////// List of pairs of storename, and items 
        public void UpdateSavedItemsList(int id, List<Pair<Guid, List<int>>> itemsIds)
        {
            lock (stockChange)
            {
                if (savedItems.ContainsKey(id))
                    savedItems[id] = itemsIds;
                else
                {
                    savedItems.Add(id, itemsIds);
                }

                if (savedTimer.ContainsKey(id))
                {
                    savedTimer[id] = new Timer(120 * 1000); // 1000 = 1 sec

                }
                else
                {
                    savedTimer.Add(id, new Timer(120 * 1000));
                }

                if (savedItemsRestore.ContainsKey(id))
                {
                    //savedItemsRestore[id] = new Dictionary<string, List<Pair<int, int>>>();
                    // restore stock to original and then update based on new selection in cart
                    foreach (Guid storeId in savedItemsRestore[id].Keys)
                    {
                        foreach (Pair<int, int> storeItem in savedItemsRestore[id][storeId])
                        {
                            storeManager.GetStoreById(storeId).AddQuantity(storeItem.First, storeItem.Second);
                        }
                    }

                }
                //else
                //{
                savedItemsRestore.Add(id, new Dictionary<Guid, List<Pair<int, int>>>());
                //}

                // get user shopping cart inventory and stock and reduce all stock and save the reduced value
                ShoppingCart cart = userManager.GetShoppingCartById(id).Value;
                foreach (ShoppingBasket basket in cart.GetShoppingBaskets())
                {
                    foreach (Pair<Guid, List<int>> item in itemsIds)
                    {
                        if (item.First.Equals(basket.GetStoreId()))
                        {
                            foreach (int itemId in item.Second)
                            {

                                if (savedItemsRestore[id].ContainsKey(basket.GetStoreId()))
                                {

                                }
                                else
                                {
                                    storeManager.GetStoreById(basket.GetStoreId()).RemoveQuantity(itemId, basket.GetItemStock(itemId));

                                    if (savedItemsRestore[id].ContainsKey(basket.GetStoreId()))
                                    {
                                        savedItemsRestore[id][basket.GetStoreId()].Add(new Pair<int, int>(itemId, basket.GetItemStock(itemId)));
                                    }
                                    else
                                    {
                                        savedItemsRestore[id].Add(basket.GetStoreId(), new List<Pair<int, int>>());
                                        savedItemsRestore[id][basket.GetStoreId()].Add(new Pair<int, int>(itemId, basket.GetItemStock(itemId)));
                                    }

                                }
                            }
                        }
                    }
                }

                savedItemsUserId.Enqueue(id);
                savedTimer[id].Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                savedTimer[id].Start();

                if (savedPurchaseResult.ContainsKey(id))
                    savedPurchaseResult[id] = false;
                else
                    savedPurchaseResult.Add(id, false);

            }
        }




        public Response PurchaseCart(int id, string paymentDetails)
        {
            // need ToDo implement this function, not implemented yet

            //for tests:
            // option 1 :demonstrates succesfull purchase
            // need to update this field after succsesful purchase
            savedPurchaseResult[id] = true;
            return new Response();
            // option 2
            //if purchase failed then do not update this field :
            //savedPurchaseResult[id] because its current value is false

            //throw new NotImplementedException();
        }


    }
}