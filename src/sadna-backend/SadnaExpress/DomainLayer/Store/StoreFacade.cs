using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;
using SadnaExpress.ServiceLayer;

namespace SadnaExpress.DomainLayer.Store
{
    public class StoreFacade : IStoreFacade
    {
        private ConcurrentDictionary<Guid, Store> stores;
        private ConcurrentBag<Review> reviews;
        private bool _isTSInitialized;
        private static IOrders _orders;

        public StoreFacade()
        {
            stores = new ConcurrentDictionary<Guid, Store>();
            reviews = new ConcurrentBag<Review>();
            _orders = Orders.Instance;
        }

        public StoreFacade(ConcurrentDictionary<Guid, Store> stores)
        {
            this.stores = stores;
            reviews = new ConcurrentBag<Review>();
            _orders = Orders.Instance;
        }

        public Guid OpenNewStore(string storeName)
        {
            IsTsInitialized();

            if (storeName.Length == 0)
                throw new Exception("Store name can not be empty");

            String internedKey = String.Intern(storeName.ToLower());

            lock (internedKey)
            {
                if (IsStoreNameExist(storeName))
                    throw new Exception("Store with this name already exist");

                Store store = new Store(storeName);
                stores.TryAdd(store.StoreID, store);
                Logger.Instance.Info(store.StoreID, nameof(StoreFacade) + ": " + nameof(OpenNewStore) + "store " + storeName + " opened.");
                return store.StoreID;
            }
        }

        public void CloseStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            lock (stores[storeID])
            {
                if (!stores[storeID].Active)
                    throw new Exception("Store already closed");
                stores[storeID].Active = false;
            }
            
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(CloseStore)+"store " + stores[storeID].StoreName + " closed.");
        }
        public void ReopenStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            lock (stores[storeID])
            {
                if (stores[storeID].Active)
                    throw new Exception("Store already reopend");
                stores[storeID].Active = true;
            }
            Logger.Instance.Info(nameof(StoreFacade) + ": " + nameof(ReopenStore) + "store " + stores[storeID].StoreName + " reopen.");
        }
        public void DeleteStore(Guid storeID)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            lock (stores[storeID])
            {
                stores.TryRemove(storeID, out var store);
                Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(DeleteStore)+"store " + store.StoreName + " deleted.");
            }
        }
        
        public List<Order> GetStorePurchases(Guid storeID)
        {
            IsTsInitialized();
            return _orders.GetOrdersByStoreId(storeID);
        }
        public Dictionary<Guid, List<Order>> GetAllStorePurchases()
        {
            IsTsInitialized();
            return _orders.GetStoreOrders();
        }
        public double PurchaseCart(Dictionary<Guid, Dictionary<Guid, int>> items, ref List<ItemForOrder> itemForOrders, string email)
        {
            IsTsInitialized();
            double sum = 0; 
            Dictionary<Guid, Dictionary<Guid, int>> storeUpdated = new Dictionary<Guid, Dictionary<Guid, int>>(); //store that the inventory already update
            try
            {
                foreach (Guid storeID in items.Keys)
                {
                    IsStoreExist(storeID);
                    if (!stores[storeID].Active)
                        throw new Exception($"The store: {storeID} not active");
                    if (stores[storeID].CheckPurchasePolicy(items[storeID]))
                    {
                        sum += stores[storeID].PurchaseCart(items[storeID], ref itemForOrders,email);
                        storeUpdated.Add(storeID, items[storeID]);
                    }
                    else
                    {
                        throw new Exception($"The shopping cart does not meet the conditions of the store's regulations");
                    }
                }
                return sum;
            }
            catch (Exception e)
            {
                AddItemToStores(storeUpdated);
                throw;
            }
        }

        public double GetItemAfterDiscount(Guid storeID, Item item)
        {
            return stores[storeID].GetItemAfterDiscount(item);
        }

        public List<Store> GetAllStoreInfo()
        {
            IsTsInitialized();
            return stores.Values.ToList();
        }

        public Store GetStoreInfo(Guid storeId)
        {
            IsTsInitialized();
            IsStoreExist(storeId);
            return stores[storeId];
        }

        public void CheckPurchaseConditions(Dictionary<Guid, Dictionary<Guid, int>> items)
        {
            foreach (Guid storeID in items.Keys)
            {
                IsStoreExist(storeID);
                if (!stores[storeID].Active)
                    throw new Exception($"The store: {storeID} not active");
                if (!stores[storeID].CheckPurchasePolicy(items[storeID]))
                {
                    throw new Exception($"The shopping cart does not meet the conditions of the store's regulations");
                }
            }
        }


        public Dictionary<Guid, Dictionary<Item, double>> GetCartItems(Dictionary<Guid, Dictionary<Guid, int>> cart)
        {
            Dictionary<Guid, Dictionary<Item, double>> cartItems = new Dictionary<Guid, Dictionary<Item, double>>();
            foreach (Guid storeId in cart.Keys)
            {
                IsStoreExist(storeId);
                Dictionary<Item, double> itemAfterDiscount = new Dictionary<Item, double>();
                if (stores[storeId].Active)
                {
                    itemAfterDiscount = stores[storeId].GetCartItems(cart[storeId]);
                    cartItems.Add(storeId, itemAfterDiscount);
                }
                else
                {
                    throw new Exception($"store {stores[storeId].StoreName} is closed!");
                }
            }

            return cartItems;
        }
        
        public void AddItemToStores(Dictionary<Guid, Dictionary<Guid, int>> items)
        {
            foreach (Guid storeID in items.Keys)
            {
                foreach (Guid itemID in items[storeID].Keys)
                {
                    stores[storeID].EditItemQuantity(itemID, items[storeID][itemID]);
                }
            }
        }

        public Guid AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            String itemNameLock = String.Intern(itemName);
            lock (itemNameLock)
            {
                var result = stores[storeID].AddItem(itemName, itemCategory, itemPrice, quantity);
                Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(AddItemToStore)+" added to store "+ storeID + "- "+itemName +" form category "+itemCategory + ": "+itemPrice+"X"+quantity);
                return result;
            }
        }

        public void RemoveItemFromStore(Guid storeID, Guid itemId)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].RemoveItem(itemId);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(RemoveItemFromStore)+" removed from store "+ storeID + "- "+itemId);
        }
        
        public void EditItemName(Guid storeID, Guid itemID, string name)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemName(itemID, name);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemName)+" edited item from store "+ storeID + "- "+storeID + "- "+name);
        }
        
        public void EditItemCategory(Guid storeID, Guid itemID, string category)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemCategory(itemID, category);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemCategory)+" edited category from store "+ storeID + "- "+storeID + "- "+category);

        }
        public void EditItemPrice(Guid storeID, Guid itemID, int price)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemPrice(itemID, price);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemPrice)+" edited price from store "+ storeID + "- "+storeID + "- "+price);

        }
        public void EditItemQuantity(Guid storeID, Guid itemID, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemQuantity(itemID, quantity);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItemQuantity)+" edited quantity from store "+ storeID + "- "+storeID + "- "+quantity);

        }
        public List<Item> GetItemsByName(string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            IsTsInitialized();
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                lock (store)
                {
                    if (!store.Active)
                        continue;
                    if (ratingStore != -1 && store.StoreRating != ratingStore)
                        continue;
                    Item item = store.GetItemsByName(itemName, minPrice, maxPrice, category, ratingItem);
                    if (item != null)
                        allItems.Add(item);
                }
            }
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(GetItemsByName));

            return allItems;
        }
        public List<Item> GetItemsByCategory(string category, int minPrice, int maxPrice, int ratingItem, int ratingStore)
        {
            IsTsInitialized();
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                lock (store)
                {
                    if (!store.Active)
                        continue;
                    if (ratingStore != -1 && store.StoreRating != ratingStore)
                        continue;
                    allItems.AddRange(store.GetItemsByCategory(category, minPrice, maxPrice, ratingItem));
                }
            }
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(GetItemsByCategory));
            return allItems;
        }
        public List<Item> GetItemsByKeysWord(string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore)
        {
            IsTsInitialized();
            List<Item> allItems = new List<Item>(); 
            foreach (Store store in stores.Values)
            {
                lock (store)
                {
                    if (!store.Active)
                        continue;
                    if (ratingStore != -1 && store.StoreRating < ratingStore)
                        continue;
                    allItems.AddRange(store.GetItemsByKeysWord(keyWords, minPrice, maxPrice, ratingItem, category));
                }
            }
            Logger.Instance.Info(nameof(StoreFacade)+": "+nameof(GetItemsByKeysWord));
            return allItems;
        }
        public void CleanUp()
        {
           stores.Clear();
           _orders.CleanUp();
           while (!reviews.IsEmpty)
           {
               reviews.TryTake(out _);
           }
        }

        public ConcurrentDictionary<Guid, Store> GetStores()
        {
            return stores;
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            _isTSInitialized = isInitialize;
        }

        private void IsTsInitialized()
        {
            if (_isTSInitialized == false)
                throw new Exception("Cannot preform any action because system trading is closed");
        }

        private void IsStoreExist(Guid storeID)
        {
            if (!stores.ContainsKey(storeID))
                throw new Exception("Store with this id does not exist");
        }
            

        public void WriteItemReview(Guid userID, Guid itemID, string reviewText)
        {
            IsTsInitialized();
            Guid storeID = GetItemStoreId(itemID);
            Store store = stores[storeID];
            if (reviewText == "")
                throw new Exception("review text cannot be empty");
            bool foundUserOrder = false;
            foreach (Order order in _orders.GetOrdersByUserId(userID))
                foreach (ItemForOrder item in order.ListItems)
                    if (item.ItemID == itemID)
                        foundUserOrder = true;
            if (!foundUserOrder)
                throw new Exception("user with id:" + userID + "tried writing review to item: " + itemID + " which he did not purchase before");
            reviews.Add(new Review(userID, stores[storeID], store.GetItemById(itemID), reviewText));
            NotificationSystem.Instance.NotifyObservers(storeID, "User just added a review on item "+ store.GetItemById(itemID).Name+" at store "+ store.StoreName, userID);
            Logger.Instance.Info(userID, nameof(StoreFacade)+": "+nameof(WriteItemReview) + userID +" write review to store "+storeID+" on "+itemID+"- "+ reviewText);
        }
        public List<Review> GetItemReviews(Guid itemID)
        {
            Guid storeID = GetItemStoreId(itemID);
            Store store = stores[storeID];
            List<Review> reviewsOfItem = new List<Review>();
            foreach (Review review in reviews)
                if (review.Store == store && review.Item == store.GetItemById(itemID))
                    reviewsOfItem.Add(review);
            return reviewsOfItem;
        }

        public void AddItemToCart(Guid storeID, Guid itemID, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            if (!stores[storeID].Active)
                throw new Exception($"store {stores[storeID].StoreName} is not active");
            stores[storeID].AddItemToCart(itemID, quantity);
        }

        public Item GetItemByID(Guid storeID, Guid itemID)
        {
            return stores[storeID].GetItemById(itemID);
        }

        public Store GetStore(Guid storeID)
        {
            IsStoreExist(storeID);
            return stores[storeID];
        }

        private bool IsStoreNameExist(string storeName)
        {
            foreach (Store store in stores.Values)
            {
                if (store.StoreName.ToLower() == storeName.ToLower())
                    return true;
            }
            return false;
        }

        public void SetTSOrders(IOrders orders)
        {
            _orders = orders;
        }

        public Condition GetCondition<T, M>(Guid store , T entity, string type, double value,DateTime dt=default, M entityRes=default, string typeRes=default, double valueRes=default)
        {
            return null;
        }
        

        public Condition AddDiscountCondition<T>(Guid store ,T entity, string type, double value)
        {
            IsStoreExist(store);
            Condition newCond = GetStore(store).AddCondition(entity, type, value);
            return newCond;
        }
        public DiscountPolicy.DiscountPolicy CreateSimplePolicy<T>(Guid store ,T level, int percent, DateTime startDate, DateTime endDate)
        {
            IsStoreExist(store);
            return GetStore(store).CreateSimplePolicy(level, percent, startDate, endDate);
        }

        public DiscountPolicy.DiscountPolicy CreateComplexPolicy(Guid store, string op, params object[] policys)
        {
            IsStoreExist(store);
            return GetStore(store).CreateComplexPolicy(op, policys);
        }
        
        public DiscountPolicyTree AddPolicy(Guid store, DiscountPolicy.DiscountPolicy discountPolicy)
        {
            IsStoreExist(store);
            return GetStore(store).AddPolicy(discountPolicy);
        }
        public void RemovePolicy(Guid store,DiscountPolicy.DiscountPolicy discountPolicy)
        {
            IsStoreExist(store);
            GetStore(store).RemovePolicy(discountPolicy);
        }

        public void EditItem(Guid userId, Guid storeID,Guid itemID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            IsTsInitialized();
            IsStoreExist(storeID);
            stores[storeID].EditItemQuantity(itemID, quantity);
            stores[storeID].EditItemName(itemID, itemName);
            stores[storeID].EditItemPrice(itemID, itemPrice);
            stores[storeID].EditItemCategory(itemID, itemCategory);
            Logger.Instance.Info(storeID,nameof(StoreFacade)+": "+nameof(EditItem)+" edited item from store "+ storeID + "- "+storeID );

        }

        public List<Item> GetItemsInStore(Guid storeId)
        {
            IsTsInitialized();
            IsStoreExist(storeId);
            List<Item> items = new List<Item>();
            foreach (Item item in GetStore(storeId).itemsInventory.items_quantity.Keys)
            {
                item.Quantity = GetStore(storeId).itemsInventory.items_quantity[item];
                items.Add(item);
            }
            return items;
        }

        public Condition AddCondition(Guid store ,string entity, string entityName, string type, double value, DateTime dt=default, string entityRes = default,string entityResName=default,
            string typeRes = default, double valueRes = default , string op= default, int opCond= default)
        {
            IsStoreExist(store);
            if (entity == "Item")
            {
                foreach (Item i in GetItemsInStore(store))
                {
                    if (i.Name == entityName)
                    {
                        Condition newCond = GetStore(store).AddCondition<Item>(i, type, value, dt , op , opCond);
                        if (entityResName != "")
                        {
                            foreach (Item j in GetItemsInStore(store))
                            {
                                if (j.Name == entityResName)
                                {
                                    ConditioningCondition newCondCond =  GetStore(store).AddConditioning(newCond, j , typeRes, valueRes);
                                    return newCondCond;
                                }
                            }
                        }
                        
                        return newCond;
                    }
                }
            }
            else if (entity == "Category")
            {
                Condition newCond = GetStore(store).AddCondition<string>(entityName, type, value, dt);
                if (entityResName != "")
                {
                    foreach (Item j in GetItemsInStore(store))
                    {
                        if (j.Name == entityResName)
                        {
                            ConditioningCondition newCondCond =  GetStore(store).AddConditioning(newCond, j , typeRes, valueRes);
                            return newCondCond;
                        }
                    }
                }
                return newCond; 
            }
            else if (entity == "Store")
            {
                Condition newCond = GetStore(store).AddCondition<Store>(GetStore(store), type, value, dt);
                if (entityResName != "")
                {
                    foreach (Item j in GetItemsInStore(store))
                    {
                        if (j.Name == entityResName)
                        {
                            ConditioningCondition newCondCond =  GetStore(store).AddConditioning(newCond, j , typeRes, valueRes);
                            return newCondCond;
                        }
                    }
                }
                return newCond; 
            }
            return null;
        }

        public void RemoveCondition(Guid storeID ,int condID)
        {
            IsStoreExist(storeID);
            foreach (Condition cond in GetStore(storeID).PurchasePolicyList)
            {
                if (cond.ID == condID)
                {
                    GetStore(storeID).RemoveConditionFromList(cond);
                    try
                    {
                        GetStore(storeID).RemoveCondition(cond);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public PurchaseCondition[]  GetAllConditions(Guid store)
        {
            IsStoreExist(store);
            return GetStore(store).GetAllConditions();
        }

        public void LoadData(Store store1, Store store2)
        {
            Guid i1 = store1.AddItem("Tshirt", "clothes", 99.8, 40);
            Guid i2 = store1.AddItem("Ipad", "electronic", 99.8, 2);
            store1.AddItem("Dress", "clothes", 70, 45);

            store2.AddItem("Pants", "clothes", 150, 200);
            store2.AddItem("Towel", "Home", 40, 450);
            store2.AddItem("Teddy bear toy", "children toys", 65, 120);
            store2.AddItem("mouse", "animals", 65, 0);

            stores.TryAdd(store1.StoreID, store1);
            stores.TryAdd(store2.StoreID, store2);
            
            Condition cond1 = store1.AddCondition(store1.GetItemById(i1), "min quantity", 0);
            Condition cond2 = store1.AddCondition(store1.GetItemById(i2), "min value", 0);
            store1.AddSimplePurchaseCondition(cond1);
            store1.AddSimplePurchaseCondition(cond2);
        }

        public Guid GetItemStoreId(Guid itemid)
        {
            foreach (Store store in stores.Values)
            {
                if (store.ItemExist(itemid))
                    return store.StoreID;
            }

            throw new Exception("Item does not exist");
        }

        public int GetItemByQuantity(Guid storeid, Guid itemid)
        {
            IsStoreExist(storeid);
            return stores[storeid].GetItemByQuantity(itemid);
        }

    }
}
