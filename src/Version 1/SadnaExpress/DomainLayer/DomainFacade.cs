using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using static SadnaExpress.DomainLayer.Store.DiscountPolicy;
using static SadnaExpress.DomainLayer.Store.Policy;
using Timer = System.Timers.Timer;

namespace SadnaExpress.DomainLayer
{
    public class DomainFacade
    {
        private static readonly object stockChange = new object();
        private static readonly object storeOwnerChange = new object();
        private static readonly object lockInstance = new object();

        private static IUserFacade userFacade;
        private static IStoreFacade storeFacade;

        private static List<ItemReview> ItemReviews;
        private static List<PurchaseHistory> purchaseHistories;
        // store review is the average rating of all product reviews for that store

        // list for saved items  <userId   <itemsId selected in Cart>>
        private static Dictionary<int, List<Pair<string, List<int>>>> savedItems = new Dictionary<int, List<Pair<string, List<int>>>>();

        private static Dictionary<int, Dictionary<string, List<Pair<int, int>>>> savedItemsRestore = new Dictionary<int, Dictionary<string, List<Pair<int, int>>>>();

        private static Dictionary<int, Timer> savedTimer = new Dictionary<int, Timer>();
        private static Dictionary<int, bool> savedPurchaseResult = new Dictionary<int, bool>();

        private static DomainFacade instance = null;

        private static Queue<int> savedItemsUserId = new Queue<int>();

        public DomainFacade()
        {
            userFacade = new UserFacade();
            storeFacade = new StoreFacade();

            ItemReviews = new List<ItemReview>();

            purchaseHistories = new List<PurchaseHistory>();

        }


        public static DomainFacade Instance
        {

            get
            {
                lock (lockInstance)
                {
                    if (instance == null)
                    {
                        instance = new DomainFacade();
                    }
                    return instance;
                }
            }
        }

        public static int Enter()
        {
            return userFacade.Enter();
        }

        public static void Exit(int id)
        {
            userFacade.Exit(id);
        }

        public void Register(int id, string email, string firstName, string lastName, string password)
        {
            userFacade.Register(id, email, firstName, lastName, password);
        }

        public int Login(int id, string email, string password)
        {
            return userFacade.Login(id, email, password);
        }

        public int Logout(int id)
        {
            return userFacade.Logout(id);
        }

        public void CleanUp()
        {
            userFacade.CleanUp();
            storeFacade.CleanUp();
        }

        public void InitializeTradingSystem(int id)
        {
            userFacade.InitializeTradingSystem(id);
        }

        public object GetStore(string store)
        {
            return storeFacade.GetStoreByName(store);
        }

        public void CreateStore(int id, string storeName)
        {
            storeFacade.OpenNewStore(storeName, id);
        }

        public static string GetUserById(int id)
        {
            return userFacade.GetUserNameById(id);
        }

        public void AddStoreItem(int id, string storeName, string itemName, string itemCategory, double price, int stock, Pair<int, int> discount)
        {
            // todo : need to decide to get store by id or by name
            // currently impelemnted by name
            //bool hasPermission = storeFacade.getStoreByName(storeName).HasPermissionToAdd(id);
            bool hasPermission = storeFacade.getStoreByName(storeName).HasPermissionToAdd(id);
            if (!hasPermission)
                throw new Exception("Cannot add item to store, userid: " + id + " don't have pemission to add items");

            DiscountPolicy dp;
            if (discount == null || discount.First == 0 || discount.Second == 0)
            {
                dp = new NoDiscount();
            }
            else
            {
                dp = new ItemDiscount(discount.First, discount.Second);
            }
            //if (!storeFacade.getStoreByName(storeName).addItem(itemName, itemCategory, price, stock, dp))
            //{
            //    throw new Exception("Failed to add new item to store");
            //}
            if (!storeFacade.getStoreByName(storeName).addItem(itemName, itemCategory, price, stock, dp))
            {
                throw new Exception("Failed to add new item to store");
            }


        }

        public void AddItemToCart(int id, string storeName, int itemID, int itemStock)
        {
            userFacade.AddItemCart(id, storeName, itemID, itemStock);
        }

        /////////////////////////////////////////////////// List of pairs of storename, and items 
        public void UpdateSavedItemsList(int id, List<Pair<string, List<int>>> itemsIds)
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
                    foreach (string storeName in savedItemsRestore[id].Keys)
                    {
                        foreach (Pair<int, int> storeItem in savedItemsRestore[id][storeName])
                        {
                            storeFacade.getStoreByName(storeName).AddStock(storeItem.First, storeItem.Second);
                        }
                    }

                }
                //else
                //{
                savedItemsRestore.Add(id, new Dictionary<string, List<Pair<int, int>>>());
                //}

                // get user shopping cart inventory and stock and reduce all stock and save the reduced value
                ShoppingCart cart = userFacade.getShoppingCartById(id);
                foreach (ShoppingBasket basket in cart.GetShoppingBaskets())
                {
                    foreach (Pair<string, List<int>> item in itemsIds)
                    {
                        if (item.First == basket.GetStore())
                        {
                            foreach (int itemId in item.Second)
                            {

                                if (savedItemsRestore[id].ContainsKey(basket.GetStore()))
                                {

                                }
                                else
                                {
                                    storeFacade.getStoreByName(basket.GetStore()).RemoveStock(itemId, basket.GetItemStock(itemId));

                                    if (savedItemsRestore[id].ContainsKey(basket.GetStore()))
                                    {
                                        savedItemsRestore[id][basket.GetStore()].Add(new Pair<int, int>(itemId, basket.GetItemStock(itemId)));
                                    }
                                    else
                                    {
                                        savedItemsRestore[id].Add(basket.GetStore(), new List<Pair<int, int>>());
                                        savedItemsRestore[id][basket.GetStore()].Add(new Pair<int, int>(itemId, basket.GetItemStock(itemId)));
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

        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // check after timer passes
            lock (stockChange)
            {
                int id = savedItemsUserId.Dequeue();
                if (savedPurchaseResult[id] == false)
                {
                    // need to restore stock failed Purchase
                    foreach (string storeName in savedItemsRestore[id].Keys)
                    {
                        foreach (Pair<int, int> storeItem in savedItemsRestore[id][storeName])
                        {
                            storeFacade.getStoreByName(storeName).AddStock(storeItem.First, storeItem.Second);
                        }
                    }
                    Logger.Instance.Error("No Purchase Success after saving items for 2 minutes!");
                }
                savedItemsRestore[id] = new Dictionary<string, List<Pair<int, int>>>();


            }
        }

        public void PurchaseCart(int id, string paymentDetails)
        {
            // need ToDo, not implemented yet

            //for tests:
            // option 1 :demonstrates succesfull purchase
            // need to update this field after succsesful purchase
            //savedPurchaseResult[id] = true;

            // option 2
            //if purchase failed then do not update this field :
            //savedPurchaseResult[id] because its current value is false

        }
    }
}
