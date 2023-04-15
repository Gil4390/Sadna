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
using System.Collections;
using System.Diagnostics;
using System.Net;

namespace SadnaExpress.ServiceLayer
{
    public class TradingSystem : ITradingSystem
    {
        private IStoreManager storeManager;
        private IUserManager userManager;
        // lock object for saving items 
        private static readonly object stockChange = new object();
        // fields for the saved items

        // list for saved items  <userId   <itemsId selected in Cart>>
        private static Dictionary<int, Dictionary<Guid, List<Pair<int, int>>>> savedItemsRestore = new Dictionary<int, Dictionary<Guid, List<Pair<int, int>>>>();


        // lock object for the instance
        private static readonly object lockInstance = new object();
        private static TradingSystem instance = null;

        public TradingSystem(ISupplierService supplierService=null, IPaymentService paymentService=null)
        {
            IUserFacade userFacade = new UserFacade(paymentService);
            IStoreFacade storeFacade = new StoreFacade(supplierService);
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

        public int GetAvailableQuantity(Guid storeId, int itemId)
        {
            return storeManager.GetStoreById(storeId).getItemsInventory().getItemQuantityById(itemId);
        }


        public int GetMaximumWaitServiceTime()
        {
            return 10000;
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

        public ResponseT<bool> InitializeTradingSystem(int id)
        {
            try
            {
                Logger.Instance.Info("User id: " + id + " requested to initialize trading system");
                return userManager.InitializeTradingSystem(id);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return new ResponseT<bool>(ex.Message);
            }
        }

        public ResponseT<List<S_Store>> GetAllStoreInfo(int id)
        {
            //get list of business stores and convert them to service stores
            throw new NotImplementedException();
        }

        public ResponseT<List<S_Order>> GetStorePurchases(int id, Guid storeID, string email)
        {
            return userManager.GetStorePurchases(id, storeID, email);
        }

        public ResponseT<Dictionary<Guid, List<S_Order>>> GetAllAStorePurchases(int id, string email)
        {
            return userManager.GetAllAStorePurchases(id, email);

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
            // first check if store can provide the itemAmount
            try
            {
                int storeQuantity = GetAvailableQuantity(storeID, itemID);
                if (storeQuantity < itemAmount)
                {
                    Logger.Instance.Error("cant add item to shopping basket with quantity more than the store can provide!");
                    return new Response();
                }
                return userManager.AddItemToCart(id, storeID, itemID, itemAmount);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("tried to add item to shopping basket from a diffrent Store!");
                return new Response(ex.Message);
            }
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

        public Response DeleteMember(int id, int userID)
        {
            throw new NotImplementedException();
        }
        public ResponseT<int> UpdateFirst(int id, string newFirst)
        {
            return userManager.UpdateFirst(id, newFirst);
        }

        public ResponseT<int> UpdateLast(int id, string newLast)
        {
            return userManager.UpdateLast(id, newLast);
        }

        public ResponseT<int> UpdatePassword(int id, string newPassword)
        {
            return userManager.UpdatePassword(id, newPassword);
        }

        public ResponseT<int> SetSecurityQA(int id,string q, string a)
        {
            return userManager.SetSecurityQA(id,q,a);
        }

        public void CleanUp() // for the tests
        {
            storeManager.CleanUp();
            userManager.CleanUp();
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

        public void SetPaymentService(IPaymentService paymentService)
        {
            throw new NotImplementedException();
        }


        public Response PurchaseCart(int id, string paymentDetails)
        {
            bool purchaseSuccess = false;
            // saving items for userid cart items on purchase cart action
            lock (stockChange)
            {
                if (savedItemsRestore.ContainsKey(id))
                    savedItemsRestore[id] = new Dictionary<Guid, List<Pair<int, int>>>();
                else
                    savedItemsRestore.Add(id, new Dictionary<Guid, List<Pair<int, int>>>());
                // get user shopping cart inventory and stock and reduce all stock and save the reduced value
                ShoppingCart cart = userManager.GetShoppingCartById(id).Value;
                foreach (ShoppingBasket basket in cart.GetShoppingBaskets())
                {
                    Store store = storeManager.GetStoreById(basket.GetStoreId());
                    Dictionary<int, int> itemsInBasket = basket.GetItemsInBasket();
                    foreach (int itemId in itemsInBasket.Keys)
                    {
                        store.RemoveQuantity(itemId, itemsInBasket[itemId]);

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
                Logger.Instance.Info("PurhcaseCart, saving store items for userid: " + id);
            }
            /////////////////////////// end of saving items implementation

            // todo: need to implement purchase cart action and
            // update bool purchaseSuccess to the purchase result







            // in case of purchase failure restore store items quantity
            if (purchaseSuccess.Equals(false))
            {
                lock (stockChange)
                {
                    foreach (Guid storeId in savedItemsRestore[id].Keys)
                    {
                        foreach (Pair<int, int> storeItem in savedItemsRestore[id][storeId])
                        {
                            storeManager.GetStoreById(storeId).AddQuantity(storeItem.First, storeItem.Second);
                        }
                    }
                    Logger.Instance.Info("Purchase Failed restored saved store items for userid: "+id);
                    savedItemsRestore.Remove(id);
                }
            }
            return new Response();
        }

        public bool isLogin(int idx)
        {
            return userManager.isLogin(idx);
        }

    }
}