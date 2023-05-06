using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.ServiceLayer
{
    public class ProxyBridge : ITradingSystem
    {
        private ITradingSystem _realBridge;

        public void SetBridge(ITradingSystem Implemantation)
        {
            _realBridge = Implemantation;
        }

        public ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID)
        {
            return _realBridge.GetAllStorePurchases(userID);
        }

        public void CleanUp()
        {
            _realBridge.CleanUp();
        }

        public ResponseT<Guid> Enter()
        {
            return _realBridge.Enter();
        }

        public Response Exit(Guid id)
        {
            return _realBridge.Exit(id);
        }

        public Response Register(Guid id, string email, string firstName, string lastName, string password)
        {
            return _realBridge.Register(id, email, firstName, lastName, password);
        }


        public ResponseT<Guid> Login(Guid id, string email, string password)
        {
            return _realBridge.Login(id, email, password);
        }

        public ResponseT<bool> InitializeTradingSystem(Guid id)
        {
            return _realBridge.InitializeTradingSystem(id);
        }

        public ResponseT<Guid> Logout(Guid id)
        {
            return _realBridge.Logout(id);
        }

        public ResponseT<Guid> OpenNewStore(Guid id, string storeName)
        {
            return _realBridge.OpenNewStore(id, storeName);
        }

        public ResponseT<List<Store>> GetAllStoreInfo()
        {
            return _realBridge.GetAllStoreInfo();
        }

        public ResponseT<List<Item>> GetItemsByName(Guid id, string itemName, int minPrice = 0, int maxPrice = int.MaxValue, int ratingItem = -1, string category = "", int ratingStore = -1)
        {
            return _realBridge.GetItemsByName(id, itemName);
        }
        public ResponseT<List<Item>> GetItemsByCategory(Guid userID, string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1)
        {
            return _realBridge.GetItemsByCategory(userID, category, minPrice, maxPrice, ratingItem, ratingStore);
        }

        public ResponseT<List<Item>> GetItemsByKeysWord(Guid userID, string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            return _realBridge.GetItemsByKeysWord(userID, keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
        }

        public Response EditItemFromCart(Guid id, Guid storeID, Guid itemID, int itemAmount)
        {
            return _realBridge.EditItemFromCart(id, storeID, itemID, itemAmount);
        }

        public ResponseT<ShoppingCart> GetDetailsOnCart(Guid id)
        {
            return _realBridge.GetDetailsOnCart(id);
        }

        public ResponseT<List<ItemForOrder>>  PurchaseCart(Guid id, string paymentDetails, string usersDetail)
        {
            return _realBridge.PurchaseCart(id, paymentDetails, usersDetail);
        }

        public Response WriteItemReview(Guid userID, Guid storeID, Guid itemID, string reviewText)
        {
            return _realBridge.WriteItemReview(userID, storeID, itemID, reviewText);
        }

        public ResponseT<List<Review>> GetItemReviews(Guid storeID, Guid itemID)
        {
            return _realBridge.GetItemReviews(storeID, itemID);
        }

        public Response RateItem(Guid id, int itemID, int score)
        {
            return _realBridge.RateItem(id, itemID, score);
        }

        public Response WriteMessageToStore(Guid id, Guid storeID, string message)
        {
            return _realBridge.WriteMessageToStore(id, storeID, message);
        }

        public Response ComplainToAdmin(Guid id, string message)
        {
            return _realBridge.ComplainToAdmin(id, message);
        }

        public ResponseT<List<Order>> GetPurchasesInfoUser(Guid userID)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<ItemForOrder>> GetPurchasesInfoUserOnlu(Guid userID)
        {
            throw new NotImplementedException();
        }


        public Response EditItemCategory(Guid userID,  Guid storeID, Guid itemID, string category)
        {
            return _realBridge.EditItemCategory(userID, storeID,  itemID, category);
        }

        public Response EditItemPrice(Guid userID,  Guid storeID, Guid itemID, int price)
        {
            return _realBridge.EditItemPrice(userID, storeID, itemID, price);
        }
        
        public Response EditItemName(Guid userID,  Guid storeID, Guid itemID, string name)
        {
            return _realBridge.EditItemName(userID, storeID, itemID, name);
        }
        
        public Response EditItemQuantity(Guid userID, Guid storeID, Guid itemID, int quantity)
        {
            // if you want remove put -i and to add +i
            return _realBridge.EditItemQuantity(userID, storeID, itemID, quantity);
        }

        public Response EditItem(Guid userID, Guid storeID,  Guid itemID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            throw new NotImplementedException();
        }

        public Response AppointStoreOwner(Guid id, Guid storeID, string userEmail)
        {
            return _realBridge.AppointStoreOwner(id, storeID, userEmail);
        }

        public Response AppointStoreManager(Guid id, Guid storeID, string newUserEmail)
        {
            return _realBridge.AppointStoreManager(id, storeID, newUserEmail);
        }

        public Response AddStoreManagerPermissions(Guid id, Guid storeID, string userEmail, string permission)
        {
            return _realBridge.AddStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManagerPermissions(Guid id, Guid storeID, string userEmail, string permission)
        {
            return _realBridge.RemoveStoreManagerPermissions(id, storeID, userEmail, permission);
        }

        public Response RemoveStoreManager(Guid id, Guid storeID, Guid userID)
        {
            return _realBridge.RemoveStoreManager(id, storeID, userID);
        }

        public Response RemoveStoreOwner(Guid id, Guid storeID, string userEmail)
        {
            return _realBridge.RemoveStoreOwner(id, storeID, userEmail);
        }

        public Response CloseStore(Guid id, Guid storeID)
        {
            return _realBridge.CloseStore(id, storeID);
        }

        public Response ReopenStore(Guid id, Guid storeID)
        {
            return _realBridge.ReopenStore(id, storeID);
        }

        public ResponseT<List<PromotedMember>> GetEmployeeInfoInStore(Guid id, Guid storeID)
        {
            return _realBridge.GetEmployeeInfoInStore(id, storeID);
        }
        
        public Response RemoveUserMembership(Guid userID, string email)
        {
            return _realBridge.RemoveUserMembership(userID, email);
        }
        public ResponseT<List<Order>> GetStorePurchases(Guid userID, Guid storeID)
        {
            return _realBridge.GetStorePurchases(userID, storeID);
        }
       

        public Response DeleteStore(Guid id, Guid storeID)
        {
            return _realBridge.DeleteStore(id, storeID);
        }

        public Response DeleteMember(Guid id, Guid userID)
        {
            return _realBridge.DeleteMember(id, userID);
        }

        public ResponseT<Guid> UpdateFirst(Guid id, string newFirst)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> UpdateLast(Guid id, string newLast)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> UpdatePassword(Guid id, string newPassword)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Guid> SetSecurityQA(Guid id,string q, string a)
        {
            throw new NotImplementedException();
        }

        public Response AddItemToCart(Guid id, Guid storeID, Guid itemID, int itemAmount)
        {
            return _realBridge.AddItemToCart(id, storeID, itemID, itemAmount);
        }

        public Response RemoveItemFromCart(Guid id, Guid storeID, Guid itemID)
        {
            return (_realBridge.RemoveItemFromCart(id, storeID, itemID));
        }

        public ResponseT<Guid> AddItemToStore(Guid id, Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity)
        {
            return _realBridge.AddItemToStore(id, storeID, itemName, itemCategory, itemPrice, quantity);
        }
        public Response RemoveItemFromStore(Guid id, Guid storeID, Guid itemID)
        {
            return _realBridge.RemoveItemFromStore(id, storeID, itemID);
        }

        public void SetPaymentService(IPaymentService paymentService)
        {
            _realBridge.SetPaymentService(paymentService);
        }

        public void SetSupplierService(ISupplierService supplierService)
        {
            _realBridge.SetSupplierService(supplierService);
        }

        public void SetIsSystemInitialize(bool isInitialize)
        {
            _realBridge.SetIsSystemInitialize(isInitialize);
        }

        public ResponseT<User> GetUser(Guid userID)
        {
            return _realBridge.GetUser(userID);
        }

        public ResponseT<Member> GetMember(Guid userID)
        {
            return _realBridge.GetMember(userID);
        }

        public ResponseT<ShoppingCart> GetUserShoppingCart(Guid userID)
        {
            return _realBridge.GetUserShoppingCart(userID);
        }

        public ResponseT<Store> GetStore(Guid storeID)
        {
            return _realBridge.GetStore(storeID);
        }

        public void SetTSOrders(IOrders orders)
        {
            _realBridge.SetTSOrders(orders);
        }

        public ResponseT<Item> GetItemByID(Guid storeID, Guid itemID)
        {
            return _realBridge.GetItemByID(storeID, itemID);
        }

        public ResponseT<List<Notification>> GetNotifications(Guid userID)
        {
            return _realBridge.GetNotifications(userID);
        }

        public bool IsSystemInitialize()
        {
            return _realBridge.IsSystemInitialize();
        }

        public ResponseT<Condition[]> GetAllConditions(Guid store)
        {
            throw new NotImplementedException();
        }


        public ResponseT<Condition> GetCondition<T, M>(Guid store, T entity, string type, double value, DateTime dt = default,
            M entityRes = default, string typeRes = default, double valueRes = default)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Condition> AddCondition<T, M>(Guid store, T entity, string type, double value, DateTime dt = default,
            M entityRes = default, string typeRes = default, double valueRes = default)
        {
            throw new NotImplementedException();
        }

        public void RemoveCondition<T, M>(Guid store, T entity, string type, double value, DateTime dt = default,
            M entityRes = default, string typeRes = default, double valueRes = default)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Condition> AddDiscountCondition<T>(Guid store, T entity, string type, double value)
        {
            throw new NotImplementedException();
        }

        public ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid store, T level, int percent, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public ResponseT<DiscountPolicy> CreateComplexPolicy(Guid store, string op, params object[] policys)
        {
            throw new NotImplementedException();
        }

        public ResponseT<DiscountPolicyTree> AddPolicy(Guid store, DiscountPolicy discountPolicy)
        {
            throw new NotImplementedException();
        }

        public void RemovePolicy(Guid store, DiscountPolicy discountPolicy)
        {
            throw new NotImplementedException();
        }

        public ResponseT<List<SItem>> GetCartItems(Guid userID)
        {
            return _realBridge.GetCartItems(userID);
        }

        public ResponseT<List<SItem>> GetItemsForClient(Guid userID, string keyWords, int minPrice = 0, int maxPrice = int.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1)
        {
            return _realBridge.GetItemsForClient(userID, keyWords, minPrice, maxPrice, ratingItem, category, ratingStore);
        }

        public ResponseT<ConcurrentDictionary<Guid, Store>> GetStores()
        {
            return _realBridge.GetStores();
        }

        public ResponseT<List<Member>> GetStoreOwners()
        {
            return _realBridge.GetStoreOwners();
        }

        public ResponseT<List<Member>> GetStoreOwnerOfStores(List<Guid> stores)
        {
            return _realBridge.GetStoreOwnerOfStores(stores);
        }

        public ResponseT<List<Item>> GetItemsInStore(Guid userID, Guid storeId)
        {
            return _realBridge.GetItemsInStore(userID,storeId);
        }

        public ResponseT<bool> IsAdmin(Guid userID)
        {
            return _realBridge.IsAdmin(userID);
        }

        public ResponseT<Dictionary<Guid, SPermission>> GetMemberPermissions(Guid userID)
        {
            return _realBridge.GetMemberPermissions(userID);
        }

        public ResponseT<SStore> GetStoreInfo(Guid userID, Guid storeId)
        {
            return _realBridge.GetStoreInfo(userID, storeId);
        }

        ResponseT<List<SMember>> ITradingSystem.GetMembers(Guid userID)
        {
            return _realBridge.GetMembers(userID);
        }

        ResponseT<Dictionary<Guid, List<Order>>> ITradingSystem.GetPurchasesInfoUser(Guid userID)
        {
            return _realBridge.GetPurchasesInfoUser(userID);
        }
    }
}
