using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
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
       

        public TradingSystem(ISupplierService supplierService=null, IPaymentService paymentService=null)
        {
            IUserFacade userFacade = new UserFacade(paymentService);
            IStoreFacade storeFacade = new StoreFacade(supplierService);
            storeManager = new StoreManager(userFacade, storeFacade);
            userManager = new UserManager(userFacade);
           
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

        public Response OpenNewStore(int id, string storeName)
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

        public Response AddItemToCart(int id, int itemID, int itemAmount)
        {
            throw new NotImplementedException();
        }

        public Response RemoveItemFromCart(int id, int itemID, int itemAmount)
        {
            throw new NotImplementedException();
        }

        public Response EditItemFromCart(int id, int itemID, int itemAmount)
        {
            throw new NotImplementedException();
        }

        public ResponseT<Dictionary<string, List<string>>> getDetailsOnCart()
        {
            throw new NotImplementedException();
        }

        public Response PurchaseCart(int id, string paymentDetails)
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

        public Response AddItemToStore(int id, Guid storeID, string itemName, string itemCategory, float itemPrice)
        {
            throw new NotImplementedException();
        }

        public Response RemoveItemFromStore(int id, int itemID)
        {
            throw new NotImplementedException();
        }

        public Response EditItemCategory(string storeName, string itemName, string category)
        {
            throw new NotImplementedException();
        }

        public Response EditItemPrice(string storeName, string itemName, int price)
        {
            throw new NotImplementedException();
        }

        public Response RemoveItemFromStore(int id, Guid storeID, int itemID)
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

        public void SetSupplierService(ISupplierService supplierService)
        {
            throw new NotImplementedException();
        }

        public ResponseT<bool> InitializeTradingSystem(int id)
        {
            return userManager.InitializeTradingSystem(id);
        }
    }
}