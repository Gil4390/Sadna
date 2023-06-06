using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.ServiceLayer.SModels;
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

        public ResponseT<List<SItem>> GetCartItems(Guid userID)
        {
            try
            {
                ShoppingCart shoppingCart = userFacade.GetDetailsOnCart(userID);
                Dictionary<Guid, Dictionary<Guid, int>> cart = new Dictionary<Guid, Dictionary<Guid, int>>(); 
                foreach (ShoppingBasket basket in shoppingCart.Baskets) 
                    cart.Add(basket.StoreID, basket.ItemsInBasket);
                Dictionary<Guid, Dictionary<Item, double>> cartItems = storeFacade.GetCartItems(cart);
                List<SItem> items = new List<SItem>();
                foreach(Guid storeID in cartItems.Keys)
                {
                    foreach (Item item in cartItems[storeID].Keys)
                    {
                        Dictionary<Guid, KeyValuePair<double, bool>> bids = userFacade.GetBidsOfUser(userID);
                        int quantity = storeFacade.GetItemByQuantity(storeID, item.ItemID);
                        if (!bids.ContainsKey(item.ItemID))
                            items.Add(new SItem(item, cartItems[storeID][item], storeID,
                                quantity > 0, cart[storeID][item.ItemID]));
                        else
                            items.Add(new SItem(item, cartItems[storeID][item], bids[item.ItemID], storeID,
                                quantity > 0, cart[storeID][item.ItemID]));
                    }
                }

                return new ResponseT<List<SItem>>(items);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID, nameof(StoreManager) + ": " + nameof(GetDetailsOnCart) + ": " + ex.Message);
                return new ResponseT<List<SItem>>(ex.Message);
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
        public ResponseT<List<ItemForOrder>>  PurchaseCart(Guid userID, SPaymentDetails paymentDetails, SSupplyDetails usersDetail)
        {
            // Version3 transaction
            // send list of objects as ref 
            List<ItemForOrder> itemForOrders = new List<ItemForOrder>();
            Dictionary<Guid, Dictionary<Guid, int>> cart = new Dictionary<Guid, Dictionary<Guid, int>>();
            lock (DBHandler.Instance)
            {
                try
                {
                    using (var db = DatabaseContextFactory.ConnectToDatabase())
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                //get the user cart
                                ShoppingCart shoppingCart = userFacade.GetDetailsOnCart(userID);
                                if (shoppingCart.Baskets.Count == 0)
                                    throw new Exception("Cart can't be empty");
                                // cast from shopping cart to dictionary before sending to store component.
                                foreach (ShoppingBasket basket in shoppingCart.Baskets)
                                    cart.Add(basket.StoreID, basket.ItemsInBasket);
                                // try to purchase the items. (the function update the quantity in the inventory in this function)
                                double amount = storeFacade.PurchaseCart(db, cart, ref itemForOrders, userFacade.GetUserEmail(userID));
                                int transaction_payment_id = userFacade.PlacePayment(amount, paymentDetails);
                                Dictionary<Guid, KeyValuePair<double, bool>> bids = userFacade.GetBidsOfUser(userID);
                                foreach (ItemForOrder item in itemForOrders)
                                {
                                    if (bids.ContainsKey(item.ItemID) && bids[item.ItemID].Value && bids[item.ItemID].Key < item.Price)
                                        item.Price = bids[item.ItemID].Key;
                                }
                                if (transaction_payment_id == -1)
                                {
                                    storeFacade.AddItemToStores(db, cart); // because we update the inventory we need to return them to inventory.
                                    throw new Exception("Payment operation failed");
                                }
                                int transaction_supply_id = userFacade.PlaceSupply(usersDetail);
                                if (transaction_supply_id == -1)
                                {
                                    storeFacade.AddItemToStores(db, cart); // because we update the inventory we need to return them to inventory.
                                    userFacade.CancelPayment(amount, transaction_payment_id); // because we need to refund the user
                                    throw new Exception("Supply operation failed");
                                }
                                Orders.Instance.AddOrder(userID, itemForOrders, true, db);

                                // Notify to store owners
                                foreach (ShoppingBasket basket in shoppingCart.Baskets)
                                    NotificationSystem.Instance.NotifyObservers(basket.StoreID, "New cart purchase at store " + storeFacade.GetStore(basket.StoreID).StoreName + " !", userID, db);

                                //for bar - notify a user that his purchase completed succssefully by notification
                                userFacade.NotifyBuyerPurchase(userID);

                                // delete the exist shopping cart
                                userFacade.PurchaseCart(db, userID);


                                // here we know that the purchase was completed successfully
                                // so now we commit the transaction to the database
                                db.SaveChanges(true);
                                transaction.Commit();

                            }
                            catch (Exception ex)
                            {
                                // here we know that the purchase wasn't completed successfully
                                // so now we disregard the transaction and any changes made in the database
                                transaction.Rollback();
                                Logger.Instance.Error(ex.Message);
                                return new ResponseT<List<ItemForOrder>>(ex.Message);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to Connect With Database");
                }
            }
            return new ResponseT<List<ItemForOrder>>(itemForOrders);
        }
        public ResponseT<Guid> OpenNewStore(Guid userID, string storeName)
        {
            try
            {
                userFacade.CheckIsValidMemberOperation(userID);
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
                NotificationSystem.Instance.NotifyObservers(storeID,"Store "+ storeFacade.GetStore(storeID).StoreName+" was Closed",userID);
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
        public Response WriteItemReview(Guid userID, Guid itemID, string reviewText)
        {
            try
            {
                userFacade.isLoggedIn(userID);
                storeFacade.WriteItemReview(userID, itemID, reviewText);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(WriteItemReview)+": "+ex.Message);
                return new Response(ex.Message);
            }
        }
        public ResponseT<List<SReview>> GetItemReviews(Guid itemID)
        {
            List<Review> reviews = storeFacade.GetItemReviews(itemID);
            List<SReview> sReviews = new List<SReview>();
            foreach (Review review in reviews) { 
                sReviews.Add(new SReview(review));
            }
            return new ResponseT<List<SReview>>(sReviews);
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

        public ResponseT<SBid> PlaceBid(Guid userID, Guid itemID, double price)
        {
            try
            {
                Guid storeID = storeFacade.GetItemStoreId(itemID);
                Bid bid = userFacade.PlaceBid(userID, storeID, itemID, storeFacade.GetStore(storeID).GetItemById(itemID).Name, price);
                return new ResponseT<SBid>(new SBid(bid));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(PlaceBid)+": "+ex.Message);
                return new ResponseT<SBid>(ex.Message);
            }
        }
        
        public Response ReactToBid(Guid userID, Guid itemID, Guid bidID, string bidResponse)
        {
            try
            {
                Guid storeID = storeFacade.GetItemStoreId(itemID);
                userFacade.ReactToBid(userID, storeID, bidID, bidResponse);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userID , nameof(StoreManager)+": "+nameof(ReactToBid)+": "+ex.Message);
                return new Response(ex.Message);
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
            storeFacade.CleanUp();
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
        public ResponseT<Store> GetStore(String name)
        {
            try
            {
                Store store = storeFacade.GetStore(name);
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

        public ResponseT<Condition> AddCondition(Guid userID ,Guid store ,string entity, string entityName, string type, object value, DateTime dt=default, string entityRes = default,string entityResName=default,
            string typeRes = default, double valueRes = default , string op= default, int opCond= default)
        {
            try
            {
                userFacade.hasPermissions(userID, store,new List<string> {"founder permissions", "owner permissions"});
                return new ResponseT<Condition>(storeFacade.AddCondition(store, entity, entityName, type, value, dt, op, opCond)); 
                
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(AddCondition) + ": " + ex.Message);
                return new ResponseT<Condition>(ex.Message);
            }
        }

        public Response RemoveCondition(Guid userID,Guid storeID , int condID)
        {
            try
            {
                userFacade.hasPermissions(userID, storeID,new List<string> {"founder permissions", "owner permissions"});
                storeFacade.RemoveCondition(storeID ,condID);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(RemoveCondition) + ": " + ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<SPolicy[]> GetAllConditions(Guid userID,Guid store)
        {
            try
            {
                userFacade.hasPermissions(userID, store,new List<string> {"founder permissions", "owner permissions"});
                List<SPolicy> cond = new List<SPolicy>();
                foreach (Condition condition in storeFacade.GetAllConditions(store))
                {
                    cond.Add(new SPolicy(condition.ID, condition.ToString(), true, "Condition"));
                }
                return new ResponseT<SPolicy[]>(cond.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetAllConditions) + ": " + ex.Message);
                return new ResponseT<SPolicy[]>(ex.Message);
            }
        }
        
        public ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid userID,Guid store, T level, int percent, DateTime startDate, DateTime endDate)
        {
            try
            {
                userFacade.hasPermissions(userID, store,new List<string> {"founder permissions", "owner permissions"});
                return new ResponseT<DiscountPolicy>(storeFacade.CreateSimplePolicy(store, level, percent, startDate , endDate));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(CreateSimplePolicy) + ": " + ex.Message);
                return new ResponseT<DiscountPolicy>(ex.Message);
            }
        }

        public ResponseT<DiscountPolicy> CreateComplexPolicy(Guid userID,Guid store, string op, int[] policys)
        {
            try
            {
                userFacade.hasPermissions(userID, store,new List<string> {"founder permissions", "owner permissions"});
                return new ResponseT<DiscountPolicy>(storeFacade.CreateComplexPolicy(store, op , policys));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(CreateComplexPolicy) + ": " + ex.Message);
                return new ResponseT<DiscountPolicy>(ex.Message);
            }
        }

        public ResponseT<List<SPolicy>> GetAllPolicy(Guid userID, Guid storeID)
        {
            try
            {
                userFacade.hasPermissions(userID, storeID,
                    new List<string> { "founder permissions", "owner permissions" });
                (Dictionary<DiscountPolicy, bool>, Dictionary<Condition, bool>) policies =
                    storeFacade.GetAllPolicy(storeID);
                List<SPolicy> spolicies = new List<SPolicy>();
                foreach (DiscountPolicy discount in policies.Item1.Keys)
                {
                    spolicies.Add(new SPolicy(discount, policies.Item1[discount] , "Policy"));
                }
                foreach (Condition cond in policies.Item2.Keys)
                {
                    spolicies.Add(new SPolicy(cond.ID, cond.ToString(), policies.Item2[cond] , "Condition"));
                }
                
                return new ResponseT<List<SPolicy>>(spolicies);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetAllPolicy) + ": " + ex.Message);
                return new ResponseT<List<SPolicy>>(ex.Message);
            }
        }

        public Response AddPolicy(Guid userID,Guid store, int discountPolicy)
        {
            try
            {
                userFacade.hasPermissions(userID, store,new List<string> {"founder permissions", "owner permissions"});
                storeFacade.AddPolicy(store, discountPolicy);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(AddPolicy) + ": " + ex.Message);
                return new ResponseT<DiscountPolicyTree>(ex.Message);
            }
        }

        public Response RemovePolicy(Guid userID,Guid store, int discountPolicy , string type)
        {
            try
            {
                userFacade.hasPermissions(userID, store,new List<string> {"founder permissions", "owner permissions"});
                storeFacade.RemovePolicy(store, discountPolicy , type);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(RemovePolicy) + ": " + ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<double> GetStoreRevenue(Guid userID, Guid storeID, DateTime date)
        {
            try
            {
                userFacade.hasPermissions(userID, storeID, new List<string> { "owner permissions", "founder permissions", });
                return new ResponseT<double>(Orders.Instance.GetStoreRevenue(storeID, date));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetStoreRevenue) + ": " + ex.Message);
                return new ResponseT<double>(ex.Message);
            }
        }

        public ResponseT<double> GetSystemRevenue(Guid userID, DateTime date)
        {
            try
            {
                userFacade.hasPermissions(userID, Guid.Empty, new List<string> { "system manager permissions" });
                return new ResponseT<double>(Orders.Instance.GetSystemRevenue(date));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetSystemRevenue) + ": " + ex.Message);
                return new ResponseT<double>(ex.Message);
            }
        }

        public Guid GetItemStoreId(Guid itemid)
        {
            return storeFacade.GetItemStoreId(itemid);
        }

        public ResponseT<int> GetItemQuantityInCart(Guid userID, Guid storeID, Guid itemID)
        {
            try
            {
                return new ResponseT<int> (userFacade.GetItemQuantityInCart(userID, storeID, itemID));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetItemQuantityInCart) + ": " + ex.Message);
                return new ResponseT<int>(ex.Message);
            }
        }

        public Response EditItem(Guid userId, Guid storeId, Guid itemId, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            try
            {
                userFacade.EditItem(userId, storeId);
                storeFacade.EditItem(storeId, itemId, itemName, itemCategory, itemPrice, quantity);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(userId, nameof(StoreManager) + ": " + nameof(EditItem) + ": " + ex.Message);
                return new Response(ex.Message);
            }
        }

        public ResponseT<List<Item>> GetItemsInStore(Guid userId,Guid storeId)
        {
            try
            {
                //add check from userfacade if hes premmisions (?)
                return new ResponseT<List<Item>>(storeFacade.GetItemsInStore(storeId));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetItemsInStore) + ": " + ex.Message);
                return new ResponseT<List<Item>>(ex.Message);
            }
        }

        public ResponseT<SStore> GetStoreInfo(Guid userID, Guid storeId)
        {
            try
            {
                ///maybe add here a check of credentials to userfacade? this func in called from client after we know that user has permissions on this store
                Store s = storeFacade.GetStoreInfo(storeId);
                return new ResponseT<SStore>(new SStore(s));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(GetStoreInfo) + ": " + ex.Message);
                return new ResponseT<SStore>(ex.Message);
            }
        }

        public double GetItemAfterDiscount(Guid itemStoreid, Item item)
        {
            return storeFacade.GetItemAfterDiscount(itemStoreid, item);
        }

        public Response CheckPurchaseConditions(Guid userId)
        {
            try
            {
                Dictionary<Guid, Dictionary<Guid, int>> cart = new Dictionary<Guid, Dictionary<Guid, int>>();
                ShoppingCart shoppingCart = userFacade.GetDetailsOnCart(userId);
                if (shoppingCart.Baskets.Count == 0)
                    throw new Exception("Cart can't be empty");
                foreach (ShoppingBasket basket in shoppingCart.Baskets) 
                    cart.Add(basket.StoreID, basket.ItemsInBasket);
                storeFacade.CheckPurchaseConditions(cart);
                return new Response();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(nameof(StoreManager) + ": " + nameof(CheckPurchaseConditions) + ": " + ex.Message);
                return new Response(ex.Message);
            }
        }
    }
}
